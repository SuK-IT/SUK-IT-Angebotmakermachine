using SuKer_Angebot.Bindings;
using System;
using System.Windows;
using System.Windows.Threading;


namespace SuKer_Angebot.Subwindows
{
    public partial class Editor : Window
    {
        public OfferItem Item
        {
            get
            {
                return Implementation.OfferObservable.List[Position];
            }
        }
        public int Position { get; set; }
        public DataContextImplementation Implementation { get; set; }

        public Editor()
        {
            InitializeComponent();
            AutoUpdate.Tick += AutoUpdateCallback;
            AutoUpdate.IsEnabled = true;
        }

        private double TaxRate
        {
            get
            {
                if (tax_Is19.IsChecked.Value)
                    return 1.19d;
                else if (tax_Is7.IsChecked.Value)
                    return 1.07d;
                else if (tax_IsCustomRate.IsChecked.Value)
                {
                    if (double.TryParse(tax_Rate.Text, out var TaxRate))
                        return 1d + (TaxRate / 100);
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }
        private double CalculateInHundred(double Value, double Rate)
        {
                return (Value / (100d - Rate) * 100);
        }
        private double Percent(double Value, double Rate, bool Addition)
        {
            if (Addition)
                return Value + (Value / 100 * Rate);
            else
                return Value - (Value / 100 * Rate);
        }
        private double GrossToNet(double Value)
        {
            return Value / TaxRate;
        }
        private double NetToGross(double Value)
        {
            return Value * TaxRate;
        }
        private DispatcherTimer AutoUpdate = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(1),
        };
        private void AutoUpdateCallback(object sender, EventArgs e)
        {
            #region Konvertierung von Text zu Datentypen.
            if (!double.TryParse(ek_Price.Text, out var ek_price))
                return;
            if (!int.TryParse(ek_Amount.Text, out var ek_amount))
                return;
            if (!double.TryParse(ek_Discount.Text, out var ek_discount))
                return;
            if (!double.TryParse(ek_Scam.Text, out var ek_scam))
                return;
            if (!double.TryParse(ek_Shipping.Text, out var ek_shipping))
                return;
            if (!double.TryParse(vk_Discount.Text, out var vk_discount))
                return;
            if (!double.TryParse(vk_Scam.Text, out var vk_scam))
                return;
            if (!double.TryParse(vk_Selfcost.Text, out var vk_selfcost))
                return;
            if (!double.TryParse(vk_Win.Text, out var vk_win))
                return;
            if (!double.TryParse(ek_Shipping.Text, out var taxnum))
                return;
            #endregion

            #region Einkaufskalkulation
            var Bezugspreis1 = ek_IsNet.IsChecked.Value ? (ek_price * ek_amount) : ((ek_price / TaxRate) * ek_amount);
            var Bezugspreis2 = ek_discountIsPercent.IsChecked.Value ? Percent(Bezugspreis1, ek_discount, false) : (Bezugspreis1 - ek_discount);
            var Bezugspreis3 = ek_scamIsPercent.IsChecked.Value ? Percent(Bezugspreis2, ek_scam, false) : (Bezugspreis2 - ek_scam);
            var BezugspreisT = Bezugspreis3 + ek_shipping;
            #endregion

            #region Verkaufskalkulation
            var Verkaufspreis1 = vk_selfcostIsPercent.IsChecked.Value ? Percent(BezugspreisT, vk_selfcost, true) : (BezugspreisT + vk_selfcost);
            var Verkaufspreis2 = vk_winIsPercent.IsChecked.Value ? Percent(Verkaufspreis1, vk_win, true) : (Verkaufspreis1 + vk_win);
            var Verkaufspreis3 = vk_scamIsPercent.IsChecked.Value ? CalculateInHundred(Verkaufspreis2, vk_scam) : (Verkaufspreis2 + vk_scam);
            var VerkaufspreisT = vk_discountIsPercent.IsChecked.Value ? CalculateInHundred(Verkaufspreis3, vk_discount) : (Verkaufspreis3 + vk_discount);
            var VerkaufspreisTG = VerkaufspreisT * TaxRate;
            #endregion

            var TotalCost = Math.Round(VerkaufspreisTG, 2);
            var SingleCost = Math.Round(VerkaufspreisTG / ek_amount, 2);
            var TotalTax = Math.Round(VerkaufspreisTG - VerkaufspreisT, 2);
            var SingleTax = Math.Round(SingleCost - (SingleCost / TaxRate), 2);

            p_Bezugspreis.Text = $"Bezugspreis: {Math.Round(BezugspreisT, 2)}";
            p_Einzelbezugspreis.Text = $"Einzelbezugspreis: {Math.Round(BezugspreisT / ek_amount, 2)}";
            p_Verkaufspreis.Text = $"Verkaufspreis: {TotalCost}";
            p_VerkaufspreisUSt.Text = $"Enthaltene USt.: {TotalTax}";

            p_Einzelverkaufspreis.Text = $"Einzelverkaufspreis: {SingleCost}";
            p_EinzelverkaufspreisUSt.Text = $"Enthaltene USt.: {SingleTax}";
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoUpdate.IsEnabled = false;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AutoUpdate.IsEnabled = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdatePosition();
        }
        private void UpdatePosition()
        {
            #region Konvertierung von Text zu Datentypen.
            if (!double.TryParse(ek_Price.Text, out var ek_price))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Einkaufspreis vor.");
                return;
            }
            if (!int.TryParse(ek_Amount.Text, out var ek_amount))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler bei der Menge vor.");
                return;
            }
            if (!double.TryParse(ek_Discount.Text, out var ek_discount))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Rabatt vor.");
                return;
            }
            if (!double.TryParse(ek_Scam.Text, out var ek_scam))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Skonto vor.");
                return;
            }
            if (!double.TryParse(ek_Shipping.Text, out var ek_shipping))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler bei den Bezugskosten vor.");
                return;
            }
            if (!double.TryParse(vk_Discount.Text, out var vk_discount))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Rabatt vor.");
                return;
            }
            if (!double.TryParse(vk_Scam.Text, out var vk_scam))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Skonto vor.");
                return;
            }
            if (!double.TryParse(vk_Selfcost.Text, out var vk_selfcost))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler bei den Handelskosten vor.");
                return;
            }
            if (!double.TryParse(vk_Win.Text, out var vk_win))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler beim Gewinn vor.");
                return;
            }
            if (!double.TryParse(ek_Shipping.Text, out var taxnum))
            {
                MessageBox.Show("Es ist ein Fehler beim Kalkulieren aufgetreten, womöglich liegt ein Formatierungsfehler bei der Steuerrate vor.");
                return;
            }
            #endregion

            #region Einkaufskalkulation
            var Bezugspreis1 = ek_IsNet.IsChecked.Value ? (ek_price * ek_amount) : ((ek_price / TaxRate) * ek_amount);
            var Bezugspreis2 = ek_discountIsPercent.IsChecked.Value ? Percent(Bezugspreis1, ek_discount, false) : (Bezugspreis1 - ek_discount);
            var Bezugspreis3 = ek_scamIsPercent.IsChecked.Value ? Percent(Bezugspreis2, ek_scam, false) : (Bezugspreis2 - ek_scam);
            var BezugspreisT = Bezugspreis3 + ek_shipping;
            #endregion

            #region Verkaufskalkulation
            var Verkaufspreis1 = vk_selfcostIsPercent.IsChecked.Value ? Percent(BezugspreisT, vk_selfcost, true) : (BezugspreisT + vk_selfcost);
            var Verkaufspreis2 = vk_winIsPercent.IsChecked.Value ? Percent(Verkaufspreis1, vk_win, true) : (Verkaufspreis1 + vk_win);
            var Verkaufspreis3 = vk_scamIsPercent.IsChecked.Value ? CalculateInHundred(Verkaufspreis2, vk_scam) : (Verkaufspreis2 + vk_scam);
            var VerkaufspreisT = vk_discountIsPercent.IsChecked.Value ? CalculateInHundred(Verkaufspreis3, vk_discount) : (Verkaufspreis3 + vk_discount);
            var VerkaufspreisTG = VerkaufspreisT * TaxRate;
            #endregion

            Implementation.OfferObservable.List[Position].TotalCost = Math.Round(VerkaufspreisTG, 2);
            Implementation.OfferObservable.List[Position].SingleCost = Math.Round(VerkaufspreisTG / ek_amount, 2);
            Implementation.OfferObservable.List[Position].TotalTax = Math.Round(VerkaufspreisTG - VerkaufspreisT, 2);
            Implementation.OfferObservable.List[Position].SingleTax = Math.Round(Implementation.OfferObservable.List[Position].SingleCost / TaxRate, 2);
            Implementation.OfferObservable.List[Position].Title = titlePosition_Box.Text;
            Implementation.OfferObservable.List[Position].Description = descriptionPosition_Box.Text;
            Implementation.OfferObservable.List[Position].TaxConditions.TaxRate = taxnum;

            if (tax_Is19.IsChecked.Value)
                Implementation.OfferObservable.List[Position].TaxConditions.TaxType = TaxType.Nineteen;
            if (tax_Is7.IsChecked.Value)
                Implementation.OfferObservable.List[Position].TaxConditions.TaxType = TaxType.Seven;
            if (tax_IsCustomRate.IsChecked.Value)
                Implementation.OfferObservable.List[Position].TaxConditions.TaxType = TaxType.Custom;

            Implementation.OfferObservable.List[Position].Amount = ek_amount;
            Implementation.OfferObservable.List[Position].PurchaseConditions.shippingCost = ek_shipping;
            Implementation.OfferObservable.List[Position].PurchaseConditions.buyingPrice = ek_price;
            Implementation.OfferObservable.List[Position].PurchaseConditions.buyingPriceIsNet = ek_IsNet.IsChecked.Value;
            Implementation.OfferObservable.List[Position].PurchaseConditions.scamAmount = ek_scam;
            Implementation.OfferObservable.List[Position].PurchaseConditions.scamAmountIsPercentual = ek_scamIsPercent.IsChecked.Value;
            Implementation.OfferObservable.List[Position].PurchaseConditions.discountAmount = ek_discount;
            Implementation.OfferObservable.List[Position].PurchaseConditions.discountAmountIsPercentual = ek_discountIsPercent.IsChecked.Value;

            Implementation.OfferObservable.List[Position].SalesConditions.winAmount = vk_win;
            Implementation.OfferObservable.List[Position].SalesConditions.winAmountIsPercentual = vk_selfcostIsPercent.IsChecked.Value;
            Implementation.OfferObservable.List[Position].SalesConditions.selfCost = vk_selfcost;
            Implementation.OfferObservable.List[Position].SalesConditions.selfCostIsPercentual = vk_selfcostIsPercent.IsChecked.Value;
            Implementation.OfferObservable.List[Position].SalesConditions.scamAmount = vk_scam;
            Implementation.OfferObservable.List[Position].SalesConditions.scamAmountIsPercentual = vk_scamIsPercent.IsChecked.Value;
            Implementation.OfferObservable.List[Position].SalesConditions.discountAmount = vk_discount;
            Implementation.OfferObservable.List[Position].SalesConditions.discountAmountIsPercentual = vk_discountIsPercent.IsChecked.Value;
        }
        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            #region Load data from existing Item, if any exist.

            //Information Block
            currentPosition_Block.Text = $"Ausgewählte Position: {Position}";
            titlePosition_Box.Text = $"{Item.Title}";
            descriptionPosition_Box.Text = $"{Item.Description}";

            //Einkaufskalkulation
            ek_Price.Text = $"{Item.PurchaseConditions.buyingPrice}";
            ek_IsNet.IsChecked = Item.PurchaseConditions.buyingPriceIsNet;
            ek_Amount.Text = $"{Item.Amount}";
            ek_Discount.Text = $"{Item.PurchaseConditions.discountAmount}";
            ek_Scam.Text = $"{Item.PurchaseConditions.scamAmount}";
            ek_scamIsPercent.IsChecked = Item.PurchaseConditions.scamAmountIsPercentual;
            ek_discountIsPercent.IsChecked = Item.PurchaseConditions.discountAmountIsPercentual;
            ek_Shipping.Text = $"{Item.PurchaseConditions.shippingCost}";

            //Verkaufskalkulation
            vk_Selfcost.Text = $"{Item.SalesConditions.selfCost}";
            vk_selfcostIsPercent.IsChecked = Item.SalesConditions.selfCostIsPercentual;
            vk_Win.Text = $"{Item.SalesConditions.winAmount}";
            vk_winIsPercent.IsChecked = Item.SalesConditions.winAmountIsPercentual;
            vk_Discount.Text = $"{Item.SalesConditions.discountAmount}";
            vk_discountIsPercent.IsChecked = Item.SalesConditions.discountAmountIsPercentual;
            vk_Scam.Text = $"{Item.SalesConditions.scamAmount}";
            vk_scamIsPercent.IsChecked = Item.SalesConditions.scamAmountIsPercentual;

            //Steuersatz
            switch (Item.TaxConditions.TaxType)
            {
                case TaxType.Nineteen:
                    {
                        tax_Is19.IsChecked = true;
                    }
                    break;
                case TaxType.Seven:
                    {
                        tax_Is7.IsChecked = true;
                    }
                    break;
                case TaxType.Custom:
                    {
                        tax_IsCustomRate.IsChecked = true;
                    }
                    break;
            }
            tax_Rate.Text = $"{Item.TaxConditions.TaxRate}";
            #endregion
        }
    }
}
