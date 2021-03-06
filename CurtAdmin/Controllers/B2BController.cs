﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CurtAdmin.Models;
using CurtAdmin.Models.B2b;

namespace CurtAdmin.Controllers {
    public class B2BController : BaseController {

        //////////////==  View Table Pages  ==/////////////////
        public ActionResult Index() {
            return View();
        }

        public ActionResult ViewCerts() // view certs
        {
            List<B2BCertificate> listOfCerts = B2B.getCertificates();
            ViewBag.listOfCerts = listOfCerts;

            return View();
        }

        public ActionResult ViewCats(int id) {
            List<CurtAdmin.B2BCategory> listOfCats = B2B.getCategories(id);
            ViewBag.certID = id;
            ViewBag.listOfCats = listOfCats;
            return View();
        }

        public ActionResult ViewLessons(int id) {
            List<CurtAdmin.B2BLesson> listOfLessons = B2B.getLessons(id);
            ViewBag.listOfLessons = listOfLessons;
            int catID = id;
            ViewBag.cat = B2B.getCategory(catID);
            ViewBag.catID = id;
            return View();
        }

        public ActionResult ViewVideos(int id) {
            int lessonID = id;

            ViewBag.lesson = B2B.getLesson(lessonID);

            return View();
        }

        public ActionResult ViewTests(int id) {
            int lessonID = id;
            List<CurtAdmin.B2BTest> listOfTests = B2B.getTests(lessonID);
            B2BLesson lesson = B2B.getLesson(lessonID);
            ViewBag.listOfTests = listOfTests;
            ViewBag.cat = lesson.B2BCategory;
            ViewBag.catID = lesson.catID;
            ViewBag.lessonID = lessonID;
            return View();
        }

        public ActionResult ViewQuestions(int id) // view questions as well
        {
            int testID = id;
            CurtAdmin.B2BTest test = B2B.getTest(testID);
            ViewBag.test = test;
            return View();
        }

        public ActionResult ViewAnswers(int id) {
            int questionID = id;
            B2BQuestion question = B2B.getQuestion(questionID);

            List<B2BAnswer> listOfAnswers = question.B2BAnswers.ToList<B2BAnswer>();
            ViewBag.listOfAnswers = listOfAnswers;
            ViewBag.question = question;
            return View();
        }

        // View B2B customers
        public ActionResult ViewCustomers() {
            List<Customer> listOfCustomers = new List<Customer>();
            string err = "";
            try {
                listOfCustomers = B2B.getB2BCustomers();
            } catch (Exception e) {
                err = "Could not load B2B Users: " + e.Message + " " + e.StackTrace;
            }
            ViewBag.err = err;
            ViewBag.listOfCustomers = listOfCustomers;
            return View();
        }


        // views all B2B users
        public ActionResult ViewUsers(int id = 0) {
            List<B2BFullUser> listOfB2BUsers = new List<B2BFullUser>();
            string err = "";

            try {
                if (id == 0) {
                        listOfB2BUsers = B2B.getB2BUsers(id);            
                } else {
                    listOfB2BUsers = B2B.getB2BUsers(id);
                }
            } catch (Exception e) {
                err = "Could not load B2B Users: " + e.Message + " " + e.StackTrace;
            }
            ViewBag.err = err;
            ViewBag.listOfB2BUsers = listOfB2BUsers;

            return View();
        }

        public ActionResult ViewUser(int id) {
            ViewBag.B2BUser = B2B.getB2BUser(id);
            ViewBag.B2BFullUser = B2B.getB2BFullUser(id);
            return View();
        }

        public ActionResult ViewUserLessons(int id, int certID) {
            B2BUser B2BUser = B2B.getB2BUser(id);
            ViewBag.B2BUser = B2BUser;
            ViewBag.cert = B2B.getCertificate(certID);
            ViewBag.B2BFullUser = B2B.getB2BFullUser(id);
            ViewBag.Lessons = B2BUser.B2BCompletedTests.Where(x => x.certID == certID).ToList<B2BCompletedTest>();
            return View();
        }

        /////////////////==  Add Pages  ==///////////////////////

        // add certificate
        [HttpGet]
        public ActionResult AddCert() {
            ViewBag.error = "";
            return View();
        }
        [HttpPost]
        public ActionResult AddCert(string title, string text, int reqNum, string logo, string inactive) {
            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;
            ViewBag.error = "";
            if (title != "" && text != "" && reqNum.ToString().Length > 0 && logo != "") {
                try {
                    B2B.addCert(title, text, reqNum, logo, inActive);
                    return RedirectToAction("ViewCerts");
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }
            return View();
        }
        // add category
        [HttpGet]
        public ActionResult AddCat(int id) {
            ViewBag.error = "";
            ViewBag.certID = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddCat(int id, string title, string text, string logo, string inactive) {
            ViewBag.error = "";
            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;
            int certID = id;
            if (title != "" && text != "" && certID.ToString().Length > 0 && logo != "") {
                try {
                    B2B.addCat(certID, title, text, logo, inActive);
                    return RedirectToAction("ViewCats", new { id = certID });
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }
            return View();
        }
        // add lesson
        [HttpGet]
        public ActionResult AddLesson(int id) {
            ViewBag.error = "";
            int catID = id;
            ViewBag.catID = catID;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddLesson(int id, string title, string text, string pdf, string inactive) {
            ViewBag.error = "";
            int catID = id;
            ViewBag.catID = catID;

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            if (title != "" && catID.ToString().Length > 0 && pdf != "") {
                try {
                    B2B.addLesson(catID, title, text, pdf, inActive);
                    return RedirectToAction("ViewLessons", new { id = catID });
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }
            return View();
        }

        public ActionResult AddVideo(int id) {
            ViewBag.error = "";
            ViewBag.listOfVideoTypes = B2B.getVideoTypes();
            ViewBag.lesson = B2B.getLesson(id);
            int lessonID = id;
            return View();
        }

        [HttpPost] // handles some dynamic form fields as well
        public ActionResult AddVideo(int id, string title, string mp4, string ogg, string webm, string inactive) {
            ViewBag.error = "";
            List<B2BVideoType> listOfVideoTypes = new List<B2BVideoType>();
            int lessonID = id;

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            try {
                listOfVideoTypes = B2B.getVideoTypes();
                ViewBag.listOfVideoTypes = listOfVideoTypes;

                // handle form data and insert into database
                if (lessonID.ToString().Length > 0 && title != "" && mp4 != "") {
                    B2B.addVideo(lessonID, title, mp4, ogg, webm, inActive);
                    return RedirectToAction("ViewVideos", new { id = lessonID });
                }

            } catch (Exception e) {
                ViewBag.error = e.Message;
            }

            return View();
        }

        // add test
        [HttpGet]
        public ActionResult AddTest(int id) {
            ViewBag.error = "";
            int lessonID = id;
            ViewBag.lessonID = lessonID;
            return View();
        }
        [HttpPost]
        public ActionResult AddTest(int id, string title, string text, double minPassPercent, string inactive) {
            ViewBag.error = "";
            int lessonID = id;
            ViewBag.lessonID = lessonID;

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            if (title != "" && text != "" && lessonID.ToString().Length > 0 && minPassPercent.ToString().Length > 0) {
                try {
                    B2B.addTest(lessonID, title, text, minPassPercent, inActive);
                    return RedirectToAction("ViewTests", new { id = lessonID });
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }
            return View();
        }
        // add question
        [HttpGet]
        public ActionResult AddQuestion(int id) {
            ViewBag.error = "";
            int testID = id;
            ViewBag.testID = testID;
            return View();
        }
        [HttpPost]
        public ActionResult AddQuestion(int id, string text, string inactive) {
            ViewBag.error = "";
            int testID = id;
            ViewBag.testID = testID;

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            if (text != "" && testID.ToString().Length > 0) {
                try {
                    B2B.addQuestion(testID, text, inActive);
                    return RedirectToAction("ViewQuestions", new { id = testID });
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }

            return View();
        }
        // add answer
        [HttpGet]
        public ActionResult AddAnswer(int id) {
            ViewBag.error = "";
            int questionID = id;
            ViewBag.questionID = questionID;
            return View();
        }
        [HttpPost]
        public ActionResult AddAnswer(int id, string text, string isCorrect, string inactive) {
            ViewBag.error = "";
            int questionID = id;
            bool isCorrectAnswer = false;
            if (isCorrect == "on") { isCorrectAnswer = true; } else { isCorrectAnswer = false; }

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            ViewBag.questionID = questionID;
            if (text != "" && questionID.ToString().Length > 0) {
                try {
                    B2B.addAnswer(questionID, text, isCorrectAnswer, inActive);
                    return RedirectToAction("ViewAnswers", new { id = questionID });
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                ViewBag.error = "Please fill out all the form fields.";
            }
            return View();
        }


        /////////////////==  Edit Pages  ==///////////////////////
        [HttpGet]
        public ActionResult EditCert(int id) {
            ViewBag.error = "";
            if (id > 0) {
                // retrieve the cert with the id
                ViewBag.cert = B2B.getCertificate(id);
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditCat(int id) {
            ViewBag.error = "";
            if (id > 0) {
                ViewBag.cat = B2B.getCategory(id);
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditLesson(int id) {
            ViewBag.error = "";
            if (id > 0) {
                B2BLesson lesson = new B2BLesson();
                lesson = B2B.getLesson(id);
                ViewBag.lesson = lesson;
                ViewBag.video = lesson.B2BVideos.FirstOrDefault<B2BVideo>();
                ViewBag.pdf = lesson.B2BResources.FirstOrDefault<B2BResource>();
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditTest(int id) {
            ViewBag.error = "";
            if (id > 0) {
                ViewBag.test = B2B.getTest(id);
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditQuestion(int id) {
            ViewBag.error = "";
            if (id > 0) {
                ViewBag.question = B2B.getQuestion(id);
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditAnswer(int id) {
            ViewBag.error = "";
            if (id > 0) {
                ViewBag.answer = B2B.getAnswer(id);
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditVideo(int id) {
            try {
                ViewBag.error = "";
                if (id > 0) {
                    B2BVideo video = B2B.getVideo(id);
                    ViewBag.video = video;
                    ViewBag.lesson = B2B.getLesson(video.lessonID);
                    B2BVideoSource videoSrc = new B2BVideoSource();

                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "mp4").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    if (videoSrc != null) {
                        ViewBag.mp4 = videoSrc.filePath;
                    }
                    videoSrc = null;
                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "ogg").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    if (videoSrc != null) {
                        ViewBag.ogg = videoSrc.filePath;
                    }
                    videoSrc = null;
                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "webm").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    if (videoSrc != null) {
                        ViewBag.webm = videoSrc.filePath;
                    }
                } else {
                    return RedirectToAction("Index");
                }

            } catch (Exception e) {
                ViewBag.error = e.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditCert(int id, string title, string text, int reqNum, string logo, string inactive) {
            ViewBag.error = "";
            if (id > 0) {
                Boolean inActive = false;
                inActive = (inactive == "on") ? true : false;

                // retrieve the cert with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BCertificate cert = new B2BCertificate();
                    cert = db.B2BCertificates.Where(x => x.id == id).FirstOrDefault<B2BCertificate>();
                    cert.title = title;
                    cert.text = text;
                    cert.requirementNum = reqNum;
                    cert.image_path = logo;
                    cert.inactive = inActive;
                    db.SubmitChanges();

                    ViewBag.cert = cert;
                    ViewBag.msg = "The certificate changes have been made.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditCat(int id, string title, string text, string logo, string inactive) {
            ViewBag.error = "";
            if (id > 0) {
                Boolean inActive = false;
                inActive = (inactive == "on") ? true : false;

                // retrieve the cat with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BCategory cat = new B2BCategory();
                    cat = db.B2BCategories.Where(x => x.id == id).FirstOrDefault<B2BCategory>();
                    cat.title = title;
                    cat.text = text;
                    cat.image_path = logo;
                    cat.inactive = inActive;
                    db.SubmitChanges();

                    ViewBag.cat = cat;
                    ViewBag.msg = "The category changes have been made.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditLesson(int id, string title, string text, string pdf, string inactive) {
            ViewBag.error = "";
            if (id > 0) {
                Boolean inActive = false;
                inActive = (inactive == "on") ? true : false;

                // retrieve the lesson with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BLesson lesson = new B2BLesson();
                    lesson = db.B2BLessons.Where(x => x.id == id).FirstOrDefault<B2BLesson>();
                    lesson.title = title;
                    lesson.Text = text;
                    lesson.inactive = inActive;
                    db.SubmitChanges();

                    B2BVideo videoObj = new B2BVideo();
                    videoObj = db.B2BVideos.Where(x => x.lessonID == lesson.id).FirstOrDefault<B2BVideo>();
                    db.SubmitChanges();

                    B2BResource pdfObj = new B2BResource();
                    pdfObj = db.B2BResources.Where(x => x.lessonID.Equals(lesson.id)).FirstOrDefault<B2BResource>();
                    pdfObj.file_path = pdf;
                    db.SubmitChanges();

                    ViewBag.lesson = lesson;
                    ViewBag.video = videoObj;
                    ViewBag.pdf = pdfObj;

                    ViewBag.msg = "The Lesson changes have been made.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditTest(int id, string title, string text, double minPassPercent, string inactive) {
            ViewBag.error = "";
            if (id > 0) {
                Boolean inActive = false;
                inActive = (inactive == "on") ? true : false;

                // retrieve the test with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BTest test = new B2BTest();
                    test = db.B2BTests.Where(x => x.id == id).FirstOrDefault<B2BTest>();
                    test.title = title;
                    test.text = text;
                    test.min_pass_percent = minPassPercent;
                    test.inactive = inActive;
                    db.SubmitChanges();

                    ViewBag.test = test;
                    ViewBag.msg = "The test changes have been made.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditQuestion(int id, string text, string inactive) {
            ViewBag.error = "";
            if (id > 0) {
                Boolean inActive = false;
                inActive = (inactive == "on") ? true : false;

                // retrieve the question with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BQuestion question = new B2BQuestion();
                    question = db.B2BQuestions.Where(x => x.id == id).FirstOrDefault<B2BQuestion>();
                    question.text = text;
                    question.inactive = inActive;
                    db.SubmitChanges();

                    ViewBag.question = question;
                    ViewBag.msg = "The question has been changed.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditAnswer(int id, string text, string isCorrect, string inactive) {
            ViewBag.error = "";

            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;

            bool isCorrectAnswer = false;
            if (isCorrect == "on") { isCorrectAnswer = true; } else { isCorrectAnswer = false; }
            if (id > 0) {
                // retrieve the answer with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BAnswer answer = new B2BAnswer();
                    answer = db.B2BAnswers.Where(x => x.id == id).FirstOrDefault<B2BAnswer>();
                    answer.text = text;
                    answer.isCorrect = isCorrectAnswer;
                    answer.inactive = inActive;
                    db.SubmitChanges();

                    ViewBag.answer = answer;
                    ViewBag.msg = "The answer has been changed.";
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditVideo(int id, string title, string mp4, string ogg, string webm, string inactive) {
            ViewBag.error = "";
            Boolean inActive = false;
            inActive = (inactive == "on") ? true : false;
            bool isCorrectAnswer = false;
            if (id > 0 && title != "") {
                // retrieve the video with the id
                try {
                    B2BDataContext db = new B2BDataContext();
                    B2BVideo video = db.B2BVideos.Where(x => x.id == id).FirstOrDefault<B2BVideo>();
                    video.title = title;
                    video.inactive = inActive;
                    db.SubmitChanges();

                    if (mp4.Length > 0) {
                        B2BVideoSource mp4videoSrc = new B2BVideoSource();
                        mp4videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "mp4").Select(x => x).FirstOrDefault<B2BVideoSource>();
                        if (mp4videoSrc != null) {
                            mp4videoSrc.filePath = mp4;
                            db.SubmitChanges();
                        } else {
                            B2BVideoSource newMp4Source = new B2BVideoSource();
                            newMp4Source.filePath = mp4;
                            newMp4Source.videoID = id;
                            B2BVideoType videoType = B2B.getVideoTypes().Where(x => x.type == "mp4").FirstOrDefault();
                            newMp4Source.typeID = videoType.id;
                            db.B2BVideoSources.InsertOnSubmit(newMp4Source);
                            db.SubmitChanges();
                        }
                    }

                    if (ogg.Length > 0) {
                        B2BVideoSource oggvideoSrc = new B2BVideoSource();
                        oggvideoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "ogg").Select(x => x).FirstOrDefault<B2BVideoSource>();
                        if (oggvideoSrc != null) {
                            oggvideoSrc.filePath = ogg;
                            db.SubmitChanges();
                        } else {
                            B2BVideoSource newoggSource = new B2BVideoSource();
                            newoggSource.filePath = ogg;
                            newoggSource.videoID = id;
                            B2BVideoType videoType = B2B.getVideoTypes().Where(x => x.type == "ogg").FirstOrDefault();
                            newoggSource.typeID = videoType.id;
                            db.B2BVideoSources.InsertOnSubmit(newoggSource);
                            db.SubmitChanges();
                        }
                    }

                    if (webm.Length > 0) {
                        B2BVideoSource webmvideoSrc = new B2BVideoSource();
                        webmvideoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "webm").Select(x => x).FirstOrDefault<B2BVideoSource>();
                        if (webmvideoSrc != null) {
                            webmvideoSrc.filePath = webm;
                            db.SubmitChanges();
                        } else {
                            B2BVideoSource newwebmSource = new B2BVideoSource();
                            newwebmSource.filePath = webm;
                            newwebmSource.videoID = id;
                            B2BVideoType videoType = B2B.getVideoTypes().Where(x => x.type == "webm").FirstOrDefault();
                            newwebmSource.typeID = videoType.id;
                            db.B2BVideoSources.InsertOnSubmit(newwebmSource);
                            db.SubmitChanges();
                        }
                    }

                    ViewBag.msg = "The video has been changed.";

                    ViewBag.video = video;
                    ViewBag.lesson = B2B.getLesson(video.lessonID);
                    B2BVideoSource videoSrc = new B2BVideoSource();
                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "mp4").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    ViewBag.mp4 = videoSrc.filePath;

                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "ogg").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    ViewBag.ogg = videoSrc.filePath;

                    videoSrc = video.B2BVideoSources.Where(x => x.VideoTypes.type == "webm").Select(x => x).FirstOrDefault<B2BVideoSource>();
                    ViewBag.webm = videoSrc.filePath;
                } catch (Exception e) {
                    ViewBag.error = e.Message;
                }
            } else {
                return RedirectToAction("Index");
            }
            return View();
        }


        /////////////////==  AJAX/Delete ==///////////////////////

        public string DeleteCert(int ID = 0) {
            string response = B2B.DeleteCert(ID); //returns a string
            return response;
        }
        public string DeleteCat(int ID = 0) {
            return B2B.DeleteCat(ID); //returns a string
        }
        public string DeleteLesson(int ID = 0) {
            return B2B.DeleteLesson(ID); //returns a string
        }
        public string DeleteTest(int ID = 0) {
            return B2B.DeleteTest(ID); //returns a string
        }
        public string DeleteQuestion(int ID = 0) {
            return B2B.DeleteQuestion(ID); //returns a string
        }
        public string DeleteAnswer(int ID = 0) {
            return B2B.DeleteAnswer(ID); //returns a string
        }
        public string DeleteVideo(int ID = 0) {
            return B2B.DeleteVideo(ID); // returns a string
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public string SetPlaqueStatus(int id, int userID) {
            return B2B.SetPlaqueStatus(id, userID);
        }
    }
}
