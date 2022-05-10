using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTFW.Models;

namespace LTFW.Controllers
{
    public class AllApisController : ApiController
    {
        HAHAEntities db = new HAHAEntities();

        int EXISTING_ACCOUNT = -2;
        int NOT_MATCHING_ACCOUNT = -3;
        int NOT_EXISTING_ACCOUNT = -4;
        int DB_ERROR = -1;
        int GENERAL_SUCCESSFULLY = 0;

        [HttpGet]
        [Route("api/accounts")]
        public IEnumerable<Account> getAccount()
        {
            return db.Accounts;
        }


        [HttpPost]
        [Route("api/accounts/sign-up")]
        public int addAccount(Account newAccount)
        {
            if (db.Accounts.Any(acc => acc.accountName == newAccount.accountName))
                return this.EXISTING_ACCOUNT;

            newAccount.createdAt = System.DateTime.Now;

            try
            {
                db.Accounts.SqlQuery($"insert into Accounts(userId, account, password) values('{newAccount.userId}', '{newAccount.accountName}', '{newAccount.password}')");
                db.Accounts.Add(newAccount);
                db.SaveChanges();
                return newAccount.userId;
            }
            catch
            {
                return this.DB_ERROR;
            }
        }

        [HttpGet]
        [Route("api/accounts/sign-in")]
        public int checkAccount(Account account)
        {
            Account check = db.Accounts
                                .Where(acc => 
                                        acc.accountName == account.accountName 
                                        && acc.password == account.password)
                                .FirstOrDefault();

            return check == null ? this.NOT_MATCHING_ACCOUNT : check.userId;
        }

        [HttpPost]
        [Route("api/accounts/profile")]
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

        [HttpGet]
        [Route("api/accounts/profile/{userId}")]
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

        [HttpGet]
        [Route("api/posts")]
        public IEnumerable<Post> getPosts()
        {
            return db.Posts.OrderBy(post => post.createdAt);
        }

        [HttpPost]
        [Route("api/posts")]
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

        [HttpGet]
        [Route("api/post/{postId}/comments")]
        public IEnumerable<Comment> getComments(int postId)
        {
            return db.Comments.Where(cmt => cmt.postId == postId).OrderBy(cmt => cmt.createdAt);
        }

        [HttpPost]
        [Route("api/post/{postId}/comments")]
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

        [HttpGet]
        [Route("api/post/{postId}/loves")]
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
        [HttpGet]
        [Route("api/users/{userId}/notifications/")]
        public IEnumerable<Notification> getNotifications(int userId)
        {
            return db.Users.Find(userId).Notifications.OrderBy(noti => noti.createdAt);
        }

    }
}
