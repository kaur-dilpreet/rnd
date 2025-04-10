namespace Reports.Core.Domain.Models
{
    public class ViewModel
    {
        public override string ToString()
        {
            if (this != null)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            return base.ToString();
        }
    }
}
