using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTFW.Models;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LTFW.Controllers
{

    [EnableCors(origins: "http://localhost:63342/", headers: "*", methods: "*")]
    public class AllApisController : ApiController
    {
        HIHIEntities db = new HIHIEntities();

        int EXISTING_ACCOUNT = -2;
        int NOT_MATCHING_ACCOUNT = -3;
        int NOT_EXISTING_ACCOUNT = -4;
        int DB_ERROR = -1;
        int GENERAL_SUCCESSFULLY = 0;

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/accounts")]
        public IEnumerable<Account> getAccount()
        {
            return db.Accounts;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/accounts/sign-up")]
        public HttpResponseMessage addAccount(Account newAccount)
        {
            if (db.Accounts.Any(acc => acc.accountName == newAccount.accountName)) return Request.CreateResponse(HttpStatusCode.OK, this.EXISTING_ACCOUNT);

            newAccount.createdAt = System.DateTime.Now;

            try
            {
                /*db.Accounts.SqlQuery($"insert into Accounts(userId, account, password) values('{newAccount.userId}', '{newAccount.accountName}', '{newAccount.password}')");*/
                db.Accounts.Add(newAccount);
                db.SaveChanges();
                /*return (IActionResult)Ok(newAccount.userId);*/
                return Request.CreateResponse(HttpStatusCode.OK, newAccount.userId);
            }
            catch
            {
                /*return (IActionResult)Ok(this.DB_ERROR);*/
                return Request.CreateResponse(HttpStatusCode.OK, this.DB_ERROR);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/accounts/sign-in")]
        public int checkAccount(Account account)
        {
            Account check = db.Accounts
                                .Where(acc => 
                                        acc.accountName == account.accountName 
                                        && acc.password == account.password)
                                .FirstOrDefault();

            return check == null ? this.NOT_MATCHING_ACCOUNT : check.userId;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/accounts/profile")]
        public int addProfile(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
                return this.GENERAL_SUCCESSFULLY;
            }
            catch
            {
                return this.NOT_EXISTING_ACCOUNT;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/accounts/profile/{userId}")]
        public User getProfile(int userId)
        {
            try
            {
                return db.Users.Find(userId);
            }
            catch
            {
                return null;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/posts")]
        public IEnumerable<Post> getPosts()
        {
            return db.Posts.OrderBy(post => post.createdAt);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/posts")]
        public bool addPost(Post newPost)
        {
            newPost.createdAt = System.DateTime.Now;

            try
            {
                db.Posts.Add(newPost);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/post/{postId}/comments")]
        public IEnumerable<Comment> getComments(int postId)
        {
            return db.Comments.Where(cmt => cmt.postId == postId).OrderBy(cmt => cmt.createdAt);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/post/{postId}/comments")]
        public bool addComments(int postId, Comment comment)
        {
            comment.createdAt = System.DateTime.Now;
            try
            {
                db.Comments.Add(comment);

                Notification noti = new Notification();

                noti.notifyObjId = 3;
                noti.userId = comment.userId;
                noti.theId = postId;
                noti.createdAt = System.DateTime.Now;

                db.Notifications.Add(noti);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/post/{postId}/loves")]
        public int getLoves(int postId)
        {
            return db.Loves.Where(love => love.postId == postId).Count();
        }

        /*[HttpPost]
        [Route("api/post/{postId}/loves")]
        public bool addLoves(int postId, Love love)
        {
            love.createdAt = System.DateTime.Now;
            db.Loves.Add(love);
            db.SaveChanges();
            try
            {

*//*                db.Notifications.Add(new Notification()
                {
                    notifyObjId = 3,
                    theId = postId
                }); *//*
                return true;
            }
            catch
            {
                return false;
            }
        }
*/
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/users/{userId}/notifications/")]
        public IEnumerable<Notification> getNotifications(int userId)
        {
            return db.Users.Find(userId).Notifications.OrderBy(noti => noti.createdAt);
        }

    }
}
