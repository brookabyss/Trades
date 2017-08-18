using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Trades.Models;
using System.Linq;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Trades.Controllers
{
    public class UserController : Controller
    {

        private TradeContext _context;
        public UserController(TradeContext context){
            _context=context;
        }
        // Get Auctions
        public List<Auctions> GetAuctions (){
            List<Auctions> auctions= _context.Auctions
                                    .Include(a=>a.Users).ToList();
                                    return auctions;

        }
        // Get User
        public Users GetUser(){
            int? Id=HttpContext.Session.GetInt32("UserId");
            Users CurrentUser= _context.Users.SingleOrDefault(a=>a.UsersId==(int)Id);
            return CurrentUser;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Errors=new List<string>();
            ViewBag.errors=new List<string>();
            return View();
        }

        [HttpPost]
        [Route("user/Register")]
        public IActionResult Register(UsersView newUser){
            ViewBag.Errors=new List<string>();
            ViewBag.errors=new List<string>();
            System.Console.WriteLine(ModelState.IsValid);
           
            if(ModelState.IsValid){
                 Users Usercheck= _context.Users.SingleOrDefault(user=>user.Username==newUser.Username);
                 if(Usercheck==null){
                     System.Console.WriteLine(newUser.FirstName);
                    Users createdUser= new Users{
                        FirstName=newUser.FirstName,
                        LastName=newUser.LastName,
                        Username=newUser.Username,
                        Password= newUser.Password,
                        Wallet=1000,
                        CreatedAt= DateTime.Now,
                        UpdatedAt=DateTime.Now,
                    };
                    _context.Users.Add(createdUser);
                    _context.SaveChanges();
                    Users ReturnedUser = _context.Users.SingleOrDefault(user => user.Username == createdUser.Username);
                    System.Console.WriteLine($"Email from returned {ReturnedUser.Username}");
                    HttpContext.Session.SetInt32("UserId",(int)ReturnedUser.UsersId);  
                    // ViewBag.Auctions= GetAuctions();
                    // ViewBag.User=GetUser();
                    // return View("Show");
                    return RedirectToAction("Show");

                 }
                 else{
                     ViewBag.errors.Add("Username already exits");
                     return View("Index");
                 }
                    
        }
            else{
                ViewBag.Errors=ModelState.Values;
                return View("Index");
            }
        }


        [HttpPost]
        [Route("user/Login'")]
        public IActionResult Login(string Username, string Password){
            ViewBag.Errors=new List<string>();
            ViewBag.errors=new List<string>();
            int errors= 0;
            if(Username.Length<3 || Username.Length>20){
                ViewBag.errors.Add("Username length has to be betweeen 3 and 20 charcters");
                errors+=1;
            }
            if (Password.Length<8){
                ViewBag.errors.Add("Username or password is incorrect");
                errors+=1;
            }
            if(errors!=0){
                 
                return View("Index");
            }
                Users ReturnedUser= _context.Users.SingleOrDefault(user=>user.Username==Username);
                if(ReturnedUser.Password==Password){
                    HttpContext.Session.SetInt32("UserId",(int)ReturnedUser.UsersId);
                    // ViewBag.Auctions= GetAuctions();
                    // ViewBag.User=GetUser();
                    // return View("Show");
                    return RedirectToAction("Show");
               }
               else{
                   ViewBag.errors.Add("Username or password is incorrect");
                   return View("Index");
               }
            }
              
            
        

        [HttpGet]
        [Route("Auctions/show")]
        public IActionResult Show(){
           int? Id=HttpContext.Session.GetInt32("UserId");
           if(Id!=null){
              var auctions= GetAuctions().OrderBy(a=>a.EndDate);
              ViewBag.Auctions= auctions;
             ViewBag.User=GetUser(); 
             return View("Show");
           }
           else{
               return RedirectToAction("Index");
           }
           
        }
        // To new auction page
        [HttpGet]
        [Route("Auctions/new")]
        public IActionResult ToAuction(){
            ViewBag.Errors=new List<string>();
            ViewBag.errors=new List<string>();
            return View("New");
        }

        // create auction
        [HttpPost]
        [Route("Auctions/create")]
        public IActionResult CreateAuction(AuctionsView newAuct){
            int? Id=HttpContext.Session.GetInt32("UserId");
            if(ModelState.IsValid){
                Auctions auction= new Auctions{
                    UsersId=(int)Id,
                    ProductName= newAuct.ProductName,
                    StartingBid= newAuct.StartingBid,
                    Description= newAuct.Description,
                    EndDate=newAuct.EndDate,
                    CreatedAt= DateTime.Now,
                    UpdatedAt=DateTime.Now,
                };
                 _context.Auctions.Add(auction);
                 _context.SaveChanges();
                return RedirectToAction("Show");
            }
            else{
                ViewBag.Errors= ModelState.Values;
                return View("New");
            }
            
            return View("New");
        }

        // Show Auction
        [HttpGet]
        [Route("Auction/{auctionId}")]
        public IActionResult ShowAuction(int auctionId){
            ViewBag.errors= new List<string>();
            System.Console.WriteLine("Heelo show");
            Auctions auction= _context.Auctions
                            .Include(a=>a.Users).
                            SingleOrDefault(a =>  a.AuctionsId==auctionId);
                            ViewBag.Auction= auction;
            Bids bid= _context.Bids
                    .Include(a=>a.Auctions)
                    .Include(a=>a.Users)
                    .OrderByDescending(a=>a.CreatedAt)
                    .Where(a=>a.AuctionsId==auction.AuctionsId).First();
            List<Bids> bids= new List<Bids>();
            bids.Add(bid);
            ViewBag.Bids=bids;
            ViewBag.CurrentBid= bid;
            return View("Auction");

        }

        // bid 
        [HttpPost]
        [Route("Auction/bid/{auctionId}")]
        public IActionResult CreateBid(int auctionId, double Amount)
        {
            ViewBag.errors=new List<string>();
            if(Amount <= 0 ){
                ViewBag.errors.Add("Bid has to be higher than 0.");
                return View("Auction");
            }
            Users currentUser= GetUser();
            Auctions auction= _context.Auctions
                            .Include(a=>a.Users).
                            SingleOrDefault(a =>  a.AuctionsId==auctionId);
                            ViewBag.Auction= auction;
            if (auction.StartingBid > currentUser.Wallet){
                ViewBag.errors.Add("Insufficient Funds, can't bid");
                return View("Auction");
            }
            else if(Amount < auction.StartingBid){
                 ViewBag.errors.Add("Bid higher");
                return View("Auction");
            }
            else if(currentUser.UsersId==auction.UsersId){
                 ViewBag.errors.Add("You can't bid on your own post");
                return View("Auction");
            }
            else{
                auction.StartingBid=Amount;
                //Intentionally not decrementing wallet because we want to take it out once a sell happens not when they bid
                // currentUser.Wallet-=Amount;
                Bids newBid= new Bids{
                    Amount= Amount,
                    UsersId= currentUser.UsersId,
                    AuctionsId=auction.AuctionsId,
                    CreatedAt= DateTime.Now,
                    UpdatedAt=DateTime.Now,
                   
                };
                _context.Bids.Add(newBid);
                
                DateTime datenow= DateTime.Now;
            //    Bids bid= _context.Bids
            //                         .Include(a=>a.Auctions)
            //                         .Include(a=>a.Users)
            //                         .OrderByDescending(a=>a.CreatedAt).First();
            
                if (auction.EndDate< datenow){
                    Users Seller= _context.Users.SingleOrDefault(a=>a.UsersId==auction.UsersId);
                    Seller.Wallet+=Amount;
                    currentUser.Wallet-=Amount;
                    _context.Auctions.Remove(auction);
                }
                _context.SaveChanges();
                
                return RedirectToAction("Show");
            }
        }
        // Check Bid status 

        public bool checkbid(int Bid){
             List<Bids> bids= _context.Bids
                    .Include(a=>a.Auctions)
                    .Include(a=>a.Users)
                    .OrderByDescending(a=>a.CreatedAt)
                    .ToList();
            Bids bid=bids.First();
            DateTime datenow= DateTime.Now;
            if (bid!=null && bid.Auctions.EndDate< datenow){
                return true;
            }
            else{
                return false;
            }
            
        }

        [HttpGet]
        [Route("Auction/delete/{auctionId}")]
        public IActionResult CreateBid(int auctionId)
        {
            Auctions auction= _context.Auctions.
                            SingleOrDefault(a =>  a.AuctionsId==auctionId);
            _context.Auctions.Remove(auction);
            
            return RedirectToAction("Show");
        }

        [HttpGet]
        [Route("user/logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
