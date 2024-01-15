using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Transfers
{
  public class AddItemModel
  {
    [Required]
    public string ItemGroup { get; set; }

    [Required]
    public string Unit { get; set; }

    [Required]
    public string ProductGroup { get; set; }

    [Required]
    public string SubGroup { get; set; }
    public string ItemPart { get; set; }
    public string ItemModel { get; set; }

    [Required]
    public string ItemDescription { get; set; }
    public string ItemBrand { get; set; }
    public string Remark { get; set; }
  }
}