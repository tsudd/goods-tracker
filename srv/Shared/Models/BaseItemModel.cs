using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsTracker.Platform.Shared.Models;

public class BaseItemModel
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public string Name { get; init; } = DEFAULT_ITEM_NAME;
    public decimal? Price { get; init; }
    public decimal? DiscountPrice { get; init; }
    public int Discount { get; init; }
    public string ImgLink { get; init; } = String.Empty;
    public DateTime? FetchDate { get; init; }
    public string VendorName { get; init; } = String.Empty;
}