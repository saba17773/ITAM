using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Web.Domain.Interfaces;
using Web.Infrastructure.Transfers;

namespace Web.Domain.Services
{
  public class DatatableService : IDatatableService
  {
    private IHelperService _helperService;

    public DatatableService(IHelperService helperService)
    {
      _helperService = helperService;
    }

    public string Filter(HttpRequest req, object field)
    {
      try
      {
        var filter = new List<FilterGridModel>();
        Regex regex = new Regex(@"columns\[(\d+)\]\[search\]\[value\]");

        foreach (var item in req.Form)
        {
          Match match = regex.Match(item.Key);
          if (match.Success)
          {
            if (item.Value[0] != "")
            {
              filter.Add(new FilterGridModel
              {
                Field = GetField(req.Form[match.Value.Replace("[search][value]", "[data]")], field),
                Value = item.Value[0],
                Type = "string"
              });
            }
          }
        }

        if (filter.Count > 0)
        {
          string q = "";
          foreach (var item in filter)
          {
            if (item.Type == "date")
            {
              q += "CONVERT(VARCHAR, " + item.Field + ", 120)" + " LIKE '%" + item.Value + "%' AND ";
            }
            else
            {
              q += " " + item.Field + " LIKE '%" + item.Value + "%' AND ";
            }
          }

          return q + " 1=1 ";
        }
        else
        {
          return "1=1";
        }
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public object Format<T>(HttpRequest req, List<T> data)
    {
      try
      {
        var draw = req.Form["draw"].FirstOrDefault();
        var start = req.Form["start"].FirstOrDefault();
        var length = req.Form["length"].FirstOrDefault();
        var sortColumnDirection = req.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = req.Form["search[value]"].FirstOrDefault();
        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        int skip = start != null ? Convert.ToInt32(start) : 0;

        return new
        {
          draw = draw,
          recordsFiltered = data.Count,
          recordsTotal = data.Count,
          data = data.Skip(skip).Take(pageSize)
        };
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    public object FormatOnce<T>(List<T> data)
    {
      try
      {
        return new
        {
          draw = 1,
          recordsFiltered = data.Count,
          recordsTotal = data.Count,
          data = data
        };
      }
      catch (System.Exception)
      {

        throw;
      }
    }

    public string GetField(string column, object field)
    {
      try
      {
        var val = _helperService.GetPropertyValue(field, column);

        if (val != null)
        {
          return val.ToString();
        }

        return column;
      }
      catch (System.Exception)
      {
        throw;
      }
    }
  }
}