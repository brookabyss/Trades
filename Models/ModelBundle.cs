namespace Trades.Models
{
    public class ModelBundle
    {
        public UsersView UsersModel { get; set; }
        public AuctionsView AuctionsModel { get; set; }
        public ModelBundle(){
           UsersModel = new UsersView();
            AuctionsModel = new AuctionsView();
        }
    }
}