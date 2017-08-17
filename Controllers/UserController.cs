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
              ViewBag.Auctions= GetAuctions();
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
        [Route("Auction/show/{auctionId}")]
        public IActionResult ShowAuction(int auctionId){

            Auctions auction= _context.Auctions
                            .Include(a=>a.Users).
                            SingleOrDefault(a =>  a.AuctionsId==auctionId);
                            ViewBag.Auction= auction;
            Bids bid= _context.Bids
                    .Include(a=>a.Auctions)
                    .Include(a=>a.Users)
                    .OrderByDescending(a=>a.CreatedAt)
                    .SingleOrDefault(a=>a.AuctionsId==auction.AuctionsId);
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
            else{
                auction.StartingBid=Amount;
                Bids newBid= new Bids{
                    Amount= Amount,
                    UsersId= currentUser.UsersId,
                    AuctionsId=auction.AuctionsId,
                    CreatedAt= DateTime.Now,
                    UpdatedAt=DateTime.Now,
                   
                };
                _context.Bids.Add(newBid);
                _context.SaveChanges();
                return RedirectToAction("Show");
            }
        }



        [HttpGet]
        [Route("user/logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
