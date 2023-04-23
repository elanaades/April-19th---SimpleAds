using Simple_Ads_Auth.Data;

namespace April_19_Homework_Simple_Ads_Auth.Models
{
    public class HomePageViewModel
    {
        public List<SimpleAd> Ads { get; set; }
        public User CurrentUser { get; set; }
    }
}
