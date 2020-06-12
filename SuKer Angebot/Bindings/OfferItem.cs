using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuKer_Angebot.Bindings
{
    public class DataContextImplementation
    {
        public OfferObservable OfferObservable { get; set; }

        public int Position
        {
            get
            {
                return OfferObservable.List.Count;
            }
        }

        public DataContextImplementation()
        {
            OfferObservable = new OfferObservable();
        }
    }

    public class OfferObservable
    {
        public ObservableCollection<OfferItem> List { get; set; } = new ObservableCollection<OfferItem>();
    }

    public class OfferItem
    {
        public int Position { get; set; }
        public int Amount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double SingleCost { get; set; }
        public double TotalCost { get; set; }
        public double SingleTax { get; set; }
        public double TotalTax { get; set; }

        public TaxConditions TaxConditions { get; set; } = new TaxConditions();
        public PurchaseConditions PurchaseConditions { get; set; } = new PurchaseConditions();
        public SalesConditions SalesConditions { get; set; } = new SalesConditions();


        public OfferItem(int Position, int Amount, string Title, double SingleCost, double TotalCost)
        {
            this.Position = Position;
            this.Amount = Amount;
            this.Title = Title;
            this.SingleCost = SingleCost;
            this.TotalCost = TotalCost;
        }
    }

    public class TaxConditions
    {
        public TaxType TaxType;
        public double TaxRate;
    }
    public enum TaxType
    {
        Nineteen,
        Seven,
        Custom,
    }
    public class PurchaseConditions
    {
        public double buyingPrice;
        public bool buyingPriceIsNet;
        public double shippingCost;
        public double discountAmount;
        public bool discountAmountIsPercentual;
        public double scamAmount;
        public bool scamAmountIsPercentual;
    }
    public class SalesConditions
    {
        public double selfCost;
        public bool selfCostIsPercentual;
        public double discountAmount;
        public bool discountAmountIsPercentual;
        public double scamAmount;
        public bool scamAmountIsPercentual;
        public double winAmount;
        public bool winAmountIsPercentual;
    }
}
