using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using site.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace site.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        [HttpGet]
        public ActionResult Index(bool? firstTime)
        {
            if (!User.Identity.IsAuthenticated)// check for the user  
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.Identity.Name == "admin_spellchecker@admin.ru")
            {
                return RedirectToAction("Statistics", "Home");
            }

            string UserName = User.Identity.Name;

            System.Linq.IQueryable<Answer> userAnswers = (db.Answers
                            .Where(a => a.UserName == UserName));

            System.Linq.IQueryable<Sentence> sentencesAvailable = (db.Sentences.Where(s => s.NumberOfAnswers < 3))
                        .Except(userAnswers.Select(a => a.Task));

            if (sentencesAvailable.LongCount() == 0)
            {
                return RedirectToAction("Finish", "Home");
            }

            Sentence[] sentences;
            System.Linq.IQueryable<Sentence> sentAns0 = (sentencesAvailable.Where(s => s.NumberOfAnswers == 0));
            System.Linq.IQueryable<Sentence> sentAns1 = (sentencesAvailable.Where(s => s.NumberOfAnswers == 1));
            System.Linq.IQueryable<Sentence> sentAns2 = (sentencesAvailable.Where(s => s.NumberOfAnswers == 2));

            if (sentAns0.LongCount() != 0)
            {
                sentences = sentAns0.ToArray();
            }
            else if (sentAns1.LongCount() != 0)
            {
                sentences = sentAns1.ToArray();
            }
            else if (sentAns2.LongCount() != 0)
            {
                sentences = sentAns2.ToArray();
            }
            else
            {
                sentences = sentencesAvailable.ToArray();
            }

            Sentence sentence = sentences[random.Next(0, sentences.Length)];

            Answer answer = new Answer { Task = sentence, AnswerText = sentence.SentenceText };
            ViewBag.Thanks = firstTime;
            ViewBag.NumUserAnswers = userAnswers.LongCount();
            ViewBag.NumUserAvailableSentences = sentencesAvailable.LongCount() - 1;
            return View(answer);
        }

        [HttpPost]
        public ActionResult Index(Answer answer, string action)
        {
            if (!User.Identity.IsAuthenticated)// check for the user  
            {
                return RedirectToAction("Login", "Account");
            }

            bool isWarning = (action == "Затрудняюсь с ответом");

            Sentence task = db.Sentences.Where(s => answer.Task.ID == s.ID).First();
            task.NumberOfAnswers++;
            db.Answers.Add(new Answer
            {
                Task = task,
                AnswerText = answer.AnswerText,
                UserName = User.Identity.Name,
                IsWarning = isWarning
            });

            if (isWarning)
            {
                task.NumberOfWarnings++;
            }

            db.SaveChanges();
            return RedirectToAction("Index", new { firstTime = true });
        }


        public ActionResult About()
        {
            ViewBag.Message = "(Инфа о 'Диалоге' и том, зачем все это нужно)";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Если Вам что-то непонятно, сообщите нам!";

            return View();
        }

        public ActionResult Finish()
        {
            return View();
        }

        public struct UserAnswers
        {
            public string UserName;
            public long NumAnswers;
        }

        public ActionResult Statistics()
        {
            if (!User.Identity.IsAuthenticated)// check for the user  
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.Identity.Name != "admin_spellchecker@admin.ru")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.NumAll = db.Sentences.LongCount();
            ViewBag.NumAnswered_0 = db.Sentences.Where(s => s.NumberOfAnswers == 0).LongCount();
            ViewBag.NumAnswered_1 = db.Sentences.Where(s => s.NumberOfAnswers == 1).LongCount();
            ViewBag.NumAnswered_2 = db.Sentences.Where(s => s.NumberOfAnswers == 2).LongCount();
            ViewBag.NumAnswered_3 = db.Sentences.Where(s => s.NumberOfAnswers == 3).LongCount();
            ViewBag.NumWarnings = db.Sentences.Where(s => s.NumberOfWarnings != 0).LongCount();

            ApplicationUser[] users = db.Users.ToArray();
            UserAnswers[] userAnswers = new UserAnswers[users.Length];
            for (int i = 0; i < users.Length; i++)
            {
                string userName = users[i].UserName;
                userAnswers[i].UserName = userName;
                userAnswers[i].NumAnswers = db.Answers.Where(a => a.UserName == userName).LongCount();
            }

            return View(userAnswers);
        }

        public ActionResult Instructions()
        {
            return View();
        }
    }
}