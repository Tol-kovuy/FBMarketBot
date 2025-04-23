namespace FBMarketBot.FillingListingForm
{
    public class FormField
    {
        public string Value { get; set; }
        public string Label { get; set; }

        public FormField(string value, string label)
        {
            Value = value;
            Label = label;
        }
    }
}
