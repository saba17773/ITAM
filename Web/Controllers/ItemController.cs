using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.Domain.Interfaces;
using Web.Domain.Repositories;
using Web.Domain.Services;
using Web.Infrastructure.Entities;
using Web.Infrastructure.Transfers;

namespace Web.Controllers
{
    public class ItemController : Controller
    {
        private IUserService _userService;
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IMessageService _messageService;
        private IConfiguration _configuration;
        private IEmailService _emailService;
        private IApproveService _approveService;
        private IItemService _itemService;

        public ItemController(
          IUserService userService,
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IMessageService messageService,
          IConfiguration configuration,
          IEmailService emailService,
          IApproveService approveService,
          IItemService itemService)
        {
            _userService = userService;
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _messageService = messageService;
            _configuration = configuration;
            _emailService = emailService;
            _approveService = approveService;
            _itemService = itemService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            _userService.CanAccess("item");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ItemAx()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetItemGroup()
        {
            try
            {
                IEnumerable<ItemGroup> itemGroup;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    itemGroup = unitOfWork.Item.GetItemGroup();
                    unitOfWork.Complete();
                }

                return Json(_datatableService.FormatOnce(itemGroup.ToList()));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetItemUnit()
        {
            try
            {
                IEnumerable<ItemUnit> itemUnit;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    itemUnit = unitOfWork.Item.GetItemUnit();
                    unitOfWork.Complete();
                }

                return Json(_datatableService.FormatOnce(itemUnit.ToList()));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetItemProductGroup()
        {
            try
            {
                IEnumerable<ItemProductGroup> itemProductGroup;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    itemProductGroup = unitOfWork.Item.GetItemProductGroup();
                    unitOfWork.Complete();
                }

                return Json(_datatableService.FormatOnce(itemProductGroup.ToList()));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetItemSubGroup()
        {
            try
            {
                IEnumerable<ItemSubGroup> itemSubGroup;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    itemSubGroup = unitOfWork.Item.GetItemSubGroup();
                    unitOfWork.Complete();
                }

                return Json(_datatableService.FormatOnce(itemSubGroup.ToList()));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public ResponseModel AddItem(AddItemModel addItem)
        {
            try
            {
                var errMessage = _messageService.ErrorMessage(ModelState);
                if (errMessage.Count > 0)
                {
                    throw new Exception(errMessage[0]);
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var currentNumberSeq = unitOfWork.NumberSeq.GetNumberSeq(_configuration["Config:ItemPrefix"]);

                    if (currentNumberSeq == null)
                    {
                        throw new Exception("Error : can't get current number.");
                    }

                    currentNumberSeq.Value += 1;

                    var updateNumberSeq = unitOfWork.NumberSeq.UpdateNumberSeq(currentNumberSeq);
                    if (!updateNumberSeq)
                    {
                        throw new Exception("Update Number Seq Failed.");
                    }

                    if (addItem.ItemDescription.Trim() == "")
                    {
                        addItem.ItemDescription = ".";
                    }

                    var itemId = _itemService.GenerateItemId(_configuration["Config:ItemPrefix"], currentNumberSeq.Value);
                    var itemName = addItem.ItemPart + " " + addItem.ItemModel + " " + addItem.ItemDescription + " " + addItem.ItemBrand;

                    var newItem = new RequestItem
                    {
                        ItemId = itemId,
                        ItemName = itemName,
                        ProductGroup = addItem.ProductGroup,
                        SubGroup = addItem.SubGroup,
                        Unit = addItem.Unit,
                        ItemGroup = addItem.ItemGroup,
                        RequestBy = _userService.GetClaim().UserId,
                        RequestDate = DateTime.Now,
                        Remark = addItem.Remark,
                        SendMailItemAX = 2
                    };

                    long insertId = unitOfWork.Item.AddItem(newItem);
                    int _insertId = Convert.ToInt32(insertId);

                    var userRequestInfo = unitOfWork.User.GetUser(_userService.GetClaim().UserId);

                    if (userRequestInfo.Email == null)
                    {
                        throw new Exception("ไม่พบ Email ของผู้ขออนุมัติ Item.");
                    }

                    var userCompany = unitOfWork.Employee.GetEmployee(userRequestInfo.EmployeeId);
                    if (userCompany == null)
                    {
                        throw new Exception("Employee Not Found.");
                    }

                    var approveGroupId = unitOfWork.Approve.GetApproveGroup().Where(x => x.Slug == "ITEM_" + userCompany.Company.ToUpper()).FirstOrDefault();
                    if (approveGroupId == null)
                    {
                        throw new Exception("กรุณาตั้งค่าการ Approve ของบริษัท " + userCompany.Company.ToUpper() + " ที่เมนู Approve Setup.");
                    }

                    List<ApproveSetup> approveLists =
                      unitOfWork.Setup
                        .GetApproveSetup()
                        .Where(x => x.Active == 1 && x.ApproveGroup == approveGroupId.Id && x.CanApprove == 1)
                        .OrderBy(x => x.ApproveLevel)
                        .ToList();

                    if (approveLists.Count == 0)
                    {
                        throw new Exception("Approve List Not Found.");
                    }

                    List<ApproveTrans> approveTransData = new List<ApproveTrans>();

                    foreach (var item in approveLists)
                    {
                        approveTransData.Add(new ApproveTrans
                        {
                            ApproveId = item.Id,
                            ItemTransId = _insertId
                        });
                    }

                    long addApproveTrans = unitOfWork.Approve.GenerateApproveTrans(approveTransData);
                    if (addApproveTrans == -1)
                    {
                        throw new Exception("Generate Approve Trans Error.");
                    }

                    var approveTransSendEmail = unitOfWork.Approve.GetApproveTransEmail(_insertId);

                    if (approveTransSendEmail == null)
                    {
                        throw new Exception("ไม่พบ Email ของผู้อนุมัติ Item.");
                    }

                    string nonce = unitOfWork.Nonce.GenerateNonce();
                    long addNonce = unitOfWork.Nonce.AddNonce(new Nonce
                    {
                        NonceKey = nonce,
                        CreateDate = DateTime.Now,
                    });

                    if (addNonce == -1)
                    {
                        throw new Exception("Create nonce failed.");
                    }

                    var sendEmail = _emailService.SendEmail(
                      "มีคำร้องขออนุมัติ Item " + itemId,
                      @"เรียนคุณ " + approveTransSendEmail.Name + @"
                        <br/><br/>
                        มีรายการขออนุมติ Item ที่รอดำเนินการ : <a href='" + _configuration["Config:BaseUrl"] + @"/Item/Approve?email=" + approveTransSendEmail.Email + @"&nonce=" + nonce + @"'>คลิกที่นี่</a>
                        <br/><br/>
                        รายละเอียด : "+itemName+@"
                      ",
                      new List<string> { approveTransSendEmail.Email },
                      userRequestInfo.Email
                    );

                    if (sendEmail.Result == false)
                    {
                        throw new Exception(sendEmail.Message);
                    }
                    else
                    {
                        var sendApprove = unitOfWork.Approve.GetApproveTrans(approveTransSendEmail.Id);
                        sendApprove.SendEmailDate = DateTime.Now;
                        unitOfWork.Approve.UpdateApproveTrans(sendApprove);
                    }

                    unitOfWork.Complete();
                }

                return new ResponseModel
                {
                    Result = true,
                    Message = "Add Item Success."
                };
            }
            catch (System.Exception ex)
            {
                return new ResponseModel
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetRequestItemGrid()
        {
            try
            {
                var columns = new {
                    Approved = "R.Approved",
                    AxCreateDate = "R.AxCreateDate",
                    AxCreated = "R.AxCreated",
                    AxItemId = "R.AxItemId",
                    Id = "R.Id",
                    ItemGroup = "R.ItemGroup",
                    ItemId = "R.ItemId",
                    ItemName = "R.ItemName",
                    ProductGroup = "R.ProductGroup",
                    Remark = "R.Remark",
                    RequestBy = "E.Name",
                    RequestDate = "R.RequestDate",
                    SubGroup = "SG.DSGSUBGROUPID",
                    Unit = "R.Unit",
                    UpdateBy = "U2.Username",
                    UpdateDate = "R.UpdateDate"
                };

                var filter = _datatableService.Filter(HttpContext.Request, columns);

                IEnumerable<RequestItemGridModel> requestItem;
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    requestItem = unitOfWork.Item.GetRequestItemGrid(filter);
                    unitOfWork.Complete();

                    return Json(_datatableService.Format(HttpContext.Request, requestItem.ToList()));
                }

                
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Approve(string email, string nonce)
        {
            try
            {
                IEnumerable<ApproveListsModel> approveLists;
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(nonce))
                    {
                        throw new Exception("Email or Nonce invalid.");
                    }

                    var checkNonce =
                      unitOfWork.Nonce
                        .GetNonce()
                        .Where(x => x.Used == 0 && x.NonceKey == nonce)
                        .FirstOrDefault();

                    if (checkNonce == null)
                    {
                        throw new Exception("Nonce invalid.");
                    }

                    approveLists = unitOfWork.Approve.GetApproveLists(email).ToList();

                    unitOfWork.Complete();
                }

                ViewData["approveLists"] = approveLists;
                ViewData["email"] = email;
                ViewData["nonce"] = nonce;

                return View();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult ApproveSubmit(IFormCollection formCollection, string email, string nonce)
        {
            try
            {
                List<ApproveResultModel> listApproveResult = _approveService.GetItemApproveResult(formCollection);
                List<ApproveResultModel> result = new List<ApproveResultModel>();
                List<ApproveTransEmailModel> nextLevelData = new List<ApproveTransEmailModel>();
                List<ApproveResultModel> waitingList = new List<ApproveResultModel>();

                foreach (var item in listApproveResult.Where(x => x.ApproveResult != 0))
                {
                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                    {
                        var approveTrans = unitOfWork.Approve.GetApproveTrans(item.TransId);
                        approveTrans.Remark = item.Remark;

                        var requestTrans = _approveService.GetRequestItemTrans(approveTrans.ItemTransId);
                        requestTrans.UpdateBy = _userService.GetClaim().UserId;
                        requestTrans.UpdateDate = DateTime.Now;

                        if (item.ApproveResult == 1) // approve
                        {
                            approveTrans.ApproveBy = _userService.GetClaim().UserId;
                            approveTrans.ApproveDate = DateTime.Now;
                        }
                        else if (item.ApproveResult == 2) // reject
                        {
                            approveTrans.RejectBy = _userService.GetClaim().UserId;
                            approveTrans.RejectDate = DateTime.Now;
                            requestTrans.Approved = item.ApproveResult; // reject
                        }
                        else // 0 = user do nothing to it.
                        {
                            waitingList.Add(item);
                        }

                        var _approveSetup = _approveService.GetApproveSetup(approveTrans.ApproveId);
                        var _users = unitOfWork.User.GetUser(requestTrans.RequestBy);
                        var _employeeInfo = unitOfWork.Employee.GetEmployee(_users.EmployeeId);

                        if (_approveSetup.FinalApprove == 1) // if you are final approve
                        {
                            ResponseModel sendEmailFinaleApprove;

                            var listSendMailTo = new List<string>();

                            // Get notify
                            var approveSetupAll = unitOfWork.Setup
                                .GetApproveSetup()
                                .Where(x => x.Active == 1 
                                    && x.ApproveGroup == _approveSetup.ApproveGroup 
                                    && x.CanApprove == 0 
                                    && x.NotifyTo == 1)
                                .OrderBy(x => x.ApproveLevel)
                                .ToList();

                            if (approveSetupAll != null)
                            {
                                foreach (var approveSetup in approveSetupAll)
                                {
                                    var _userNotify = unitOfWork.User.GetUser(approveSetup.UserId);
                                    listSendMailTo.Add(_userNotify.Email);
                                }
                            }

                            listSendMailTo.Add(_users.Email);

                            if (item.ApproveResult == 1) // approve
                            {
                                requestTrans.Approved = item.ApproveResult;

                                //sendEmailFinaleApprove = _emailService.SendEmail(
                                //  $"ผลการอนุมัติขอสร้าง Item {requestTrans.ItemId}",
                                //  $"เรียนคุณ {_employeeInfo.Name}" +
                                //  $"<br/></br> <b>Item : </b> {requestTrans.ItemId} ได้รับอนุมัติแล้ว <br/>" +
                                //  $" <b>เหตุผล : </b> {item.Remark}",
                                //  listSendMailTo,
                                //  _users.Email
                                //);
                                
                            }
                            else // reject
                            {
                                sendEmailFinaleApprove = _emailService.SendEmail(
                                  $"ผลการอนุมัติขอสร้าง Item {requestTrans.ItemId}",
                                  $"เรียนคุณ {_employeeInfo.Name}" +
                                  $"<br/></br> <b>Item</b> : {requestTrans.ItemId} ไม่อนุมัติ <br/>" +
                                  $" <b>เหตุผล : </b> {item.Remark}",
                                  listSendMailTo,
                                  _users.Email
                                );

                                if (sendEmailFinaleApprove.Result == false)
                                {
                                    throw new Exception(sendEmailFinaleApprove.Message);
                                }
                            }

                           
                        }
                        else // if not final approve, send to next level
                        {
                            ResponseModel sendEmailFinaleApprove;

                            if (item.ApproveResult == 2) // reject
                            {
                                sendEmailFinaleApprove = _emailService.SendEmail(
                                  $"ผลการอนุมัติขอสร้าง Item {requestTrans.ItemId}",
                                  $"เรียนคุณ {_employeeInfo.Name}" +
                                  $"<br/></br> <b>Item</b> : {requestTrans.ItemId} ไม่อนุมัติ <br/>" +
                                  $" <b>เหตุผล : </b> {item.Remark}",
                                  new List<string> { _users.Email },
                                  _users.Email
                                );

                                if (sendEmailFinaleApprove.Result == false)
                                {
                                    throw new Exception(sendEmailFinaleApprove.Message);
                                }
                            }
                            else
                            {
                                if (item.ApproveResult != 0)
                                {
                                    var _nextLevelData = unitOfWork.Approve.GetApproveTransEmail(requestTrans.Id);
                                    var nextApproveTrans = unitOfWork.Approve.GetApproveTrans(_nextLevelData.Id);
                                    nextApproveTrans.SendEmailDate = DateTime.Now;
                                    unitOfWork.Approve.UpdateApproveTrans(nextApproveTrans);
                                }
                            }
                        }

                        unitOfWork.Approve.UpdateApproveTrans(approveTrans);
                        unitOfWork.Item.UpdateItem(requestTrans);
                        unitOfWork.Complete();
                    }
                }

                // update nonce
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = unitOfWork.Nonce.GetByNonce(nonce);
                    _nonce.Used = 1;
                    _nonce.UpdateDate = DateTime.Now;
                    unitOfWork.Nonce.UpdateNonce(_nonce);
                    unitOfWork.Complete();
                }

                // resend item undone
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveWaitingList = unitOfWork.Approve.GetApproveLists(email).ToList();
                    var approveWaiting = approveWaitingList.FirstOrDefault();
                    if (approveWaitingList.Count > 0 && approveWaiting != null)
                    {
                        string newNonce = unitOfWork.Nonce.GenerateNonce();
                        long addNonce = unitOfWork.Nonce.AddNonce(new Nonce
                        {
                            NonceKey = newNonce,
                            CreateDate = DateTime.Now,
                        });

                        var sendEmail = _emailService.SendEmail(
                             "มีคำร้องขออนุมัติ Item ที่รอการดำเนินการ",
                             @"เรียนคุณ " + approveWaiting.Name +
                             @"<br/><br/> มีรายการขออนุมติ Item ที่รอดำเนินการ : <a href='" + _configuration["Config:BaseUrl"] + @"/Item/Approve?email=" + approveWaiting.Email + @"&nonce=" + newNonce + @"'>คลิกที่นี่</a>"+
                             @"<br/><br/> รายละเอียด : "+ approveWaiting.ItemName,
                             new List<string> { approveWaiting.Email },
                             approveWaiting.Email
                           );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }

                        foreach (var item in approveWaitingList)
                        {
                            var temp = unitOfWork.Approve.GetApproveTrans(item.Id);
                            temp.SendEmailDate = DateTime.Now;
                            unitOfWork.Approve.UpdateApproveTrans(temp);
                        }

                        unitOfWork.Complete();
                    }
                }

                ViewData["SuccessMessage"] = new List<string> { "ดำเนินการเรียบร้อย" };
                return View("~/Views/Item/Index.cshtml");
            }
            catch (System.Exception ex)
            {
                ViewData["ErrorMessage"] = new List<string> { ex.Message };
                return View("~/Views/Item/Index.cshtml");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GetItemAxGrid()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {


                    var filter = _datatableService.Filter(HttpContext.Request, new
                    {
                        ItemGroup = "IT.ITEMGROUPID",
                        ItemId = "IT.ITEMID",
                        ItemName = "IT.ITEMNAME",
                        SubGroup = "SG.DSGSUBGROUPDESCRIPTION",
                        ProductGroup = "PG.DSGPRODUCTGROUPDESCRIPTION",
                        Unit = "IT.BOMUNITID"
                    });

                    var data = unitOfWork.Item.GetItemAx(filter).ToList();

                    unitOfWork.Complete();

                    return Json(_datatableService.Format(HttpContext.Request, data));
                }

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public ResponseModel CancelItem(CancelItemModel cancelItem)
        {
            try
            {
                var errMessage = _messageService.ErrorMessage(ModelState);
                if (errMessage.Count > 0)
                {
                    throw new Exception(errMessage[0]);
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                  
                    var requestTrans = _approveService.GetRequestItemTrans(cancelItem.Id);
                    requestTrans.UpdateBy = _userService.GetClaim().UserId;
                    requestTrans.UpdateDate = DateTime.Now;
                    requestTrans.Approved = 3;

                    unitOfWork.Item.UpdateItem(requestTrans);


                    unitOfWork.Complete();

                    return new ResponseModel
                    {
                        Result = true,
                        Message = "Cancel Complete"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new ResponseModel
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        [HttpPost]
        [Authorize]
        public ResponseModel EditItem(EditItemModel editItem)
        {
            try
            {
                var errMessage = _messageService.ErrorMessage(ModelState);
                if (errMessage.Count > 0)
                {
                    throw new Exception(errMessage[0]);
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var itmName = "";

                    if (editItem.EItemDescription.Trim() == "")
                    {
                        editItem.EItemDescription = ".";
                    }

                    if (editItem.EItemName == "" || editItem.EItemName == null) 
                    {
                        itmName = editItem.EItemPart + " " + editItem.EItemModel + " " + editItem.EItemDescription + " " + editItem.EItemBrand;
                    }
                    else
                    {
                        itmName = editItem.EItemName;
                    }
                    var requestTrans = _approveService.GetRequestItemTrans(editItem.EId);
                    requestTrans.ItemGroup = editItem.EItemGroup;
                    requestTrans.Unit = editItem.EUnit;
                    requestTrans.ProductGroup = editItem.EProductGroup;
                    requestTrans.ItemName = itmName;
                    requestTrans.SubGroup = editItem.ESubGroup;
                    requestTrans.Remark = editItem.ERemark;
                    requestTrans.UpdateBy = _userService.GetClaim().UserId;
                    requestTrans.UpdateDate = DateTime.Now;

                    

                    var alertEmailAll = _approveService.GetEmailAlert(editItem.EId);

                    foreach(var Email in alertEmailAll)
                    {
                        ResponseModel sendEmailAlert;
                        sendEmailAlert = _emailService.SendEmail(
                                    $"แก้ไข Item : {requestTrans.ItemId}",
                                    $"เรียนคุณ {Email.ApproveName}" +
                                    $"<br/></br> <b>Item</b> : {requestTrans.ItemId} มีการแก้ไข <br/>"+
                                    $"<b>Item Name</b>: {requestTrans.ItemName} <br/>" +
                                    $"<b>Item Group</b>: {requestTrans.ItemGroup} <br/>" +
                                    $"<b>Unit</b>: {requestTrans.Unit} <br/>" +
                                    $"<b>Product Group</b>: {requestTrans.ProductGroup} <br/>" +
                                    $"<b>Sub Group</b>: {requestTrans.SubGroup} <br/>",
                                    new List<string> { Email.Email },
                                    Email.RequesterEmail
                        );
                    }

                    unitOfWork.Item.UpdateItem(requestTrans);

                    unitOfWork.Complete();

                    return new ResponseModel
                    {
                        Result = true,
                        Message = "Edit Complete"
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new ResponseModel
                {
                    Result = false,
                    Message = ex.Message + editItem.EItemName
                };
            }
        }

        public IActionResult Sendmail()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var requestTransAll = _approveService.GetRequestItemTransAlert().ToList();
                    foreach (var trans in requestTransAll)
                    {
                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var updateTrans = _approveService.GetRequestItemTrans(trans.Id);

                            updateTrans.SendMailItemAX = 1;
                            ResponseModel sendEmailFinaleApprove;

                            sendEmailFinaleApprove = _emailService.SendEmail(
                             $"ผลการอนุมัติขอสร้าง Item : {trans.ItemId} ",
                             $"เรียนคุณ {trans.RequestBy}" +
                             $"<br/></br> <b>Item : </b> {trans.ItemId} ได้รับอนุมัติแล้ว <br/>" +
                              $"<br/></br> <b>Item AX : </b> {trans.AxItemId} : {trans.AxItemName} ได้รับอนุมัติแล้ว <br/>" +
                             $" <b>เหตุผล : </b> {trans.Message}",
                             new List<string> { trans.EmailRequester },
                             ""
                           );

                            unitOfWork2.Item.UpdateItem(updateTrans);
                            unitOfWork2.Complete();
                        }
                    }

                    unitOfWork.Complete();


                    return new JsonResult("Successfully");
                }
            }
            catch (System.Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}