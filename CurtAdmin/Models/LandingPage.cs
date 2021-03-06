﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurtAdmin {
    partial class LandingPage {
        public List<LandingPage> GetAll() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<LandingPage> landingPages = new List<LandingPage>();
            landingPages = db.LandingPages.Where(x => x.endDate > DateTime.Now).OrderBy(x => x.startDate).ToList<LandingPage>();
            return landingPages;
        }

        public List<LandingPage> GetAllPast() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<LandingPage> landingPages = new List<LandingPage>();
            landingPages = db.LandingPages.Where(x => x.endDate <= DateTime.Now).OrderBy(x => x.endDate).ToList<LandingPage>();
            return landingPages;
        }

        public LandingPage Get(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            LandingPage landingPage = new LandingPage();
            try {
                landingPage = db.LandingPages.Where(x => x.id.Equals(id)).First();
            } catch { }
            return landingPage;
        }

        public void Remove(int id) {
            CurtDevDataContext db = new CurtDevDataContext();
            LandingPage page = db.LandingPages.Where(x => x.id.Equals(id)).First();
            List<LandingPageData> datas = page.LandingPageDatas.ToList(); ;
            List<LandingPageImage> images = page.LandingPageImages.ToList();
            db.LandingPageDatas.DeleteAllOnSubmit(datas);
            db.LandingPageImages.DeleteAllOnSubmit(images);
            db.LandingPages.DeleteOnSubmit(page);
            db.SubmitChanges();
        }

        public LandingPage Save(int id, string name, int websiteID, DateTime startDate, DateTime endDate, string url = "", string content = null, string linkClasses = null, bool newWindow = false, string conversionID = null, string conversionLabel = null, string menuPosition = "top") {
            CurtDevDataContext db = new CurtDevDataContext();
            LandingPage landingPage = new LandingPage();
            try {
                if (id == 0) {
                    landingPage = new LandingPage {
                        name = name,
                        websiteID = websiteID,
                        startDate = startDate,
                        endDate = endDate,
                        url = url.Trim(),
                        pageContent = content,
                        linkClasses = (linkClasses == null) ? linkClasses : linkClasses.Trim(),
                        newWindow = newWindow,
                        conversionID = conversionID,
                        conversionLabel = conversionLabel,
                        menuPosition = menuPosition
                    };
                    db.LandingPages.InsertOnSubmit(landingPage);
                } else {
                    landingPage = db.LandingPages.Where(x => x.id.Equals(id)).First();
                    landingPage.name = name;
                    landingPage.websiteID = websiteID;
                    landingPage.startDate = startDate;
                    landingPage.endDate = endDate;
                    landingPage.url = url.Trim();
                    landingPage.pageContent = content;
                    landingPage.linkClasses = (linkClasses == null) ? linkClasses : linkClasses.Trim();
                    landingPage.newWindow = newWindow;
                    landingPage.conversionID = conversionID;
                    landingPage.conversionLabel = conversionLabel;
                    landingPage.menuPosition = menuPosition;
                }
                db.SubmitChanges();
            } catch {}

            return landingPage;
        }

        public List<LandingPageImage> GetImages(int id) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<LandingPageImage> images = new List<LandingPageImage>();
            try {
                images = db.LandingPageImages.Where(x => x.landingPageID.Equals(id)).OrderBy(x => x.sort).ToList();
            } catch {}
            return images;
        }

        public List<LandingPageImage> AddImage(int pageID, string url) {
            CurtDevDataContext db = new CurtDevDataContext();
            int sort = 1;
            try {
                sort = db.LandingPageImages.Where(x => x.landingPageID.Equals(pageID)).OrderByDescending(x => x.sort).Select(x => x.sort).First() + 1;
            } catch {}
            LandingPageImage image = new LandingPageImage {
                landingPageID = pageID,
                url = url,
                sort = sort
            };
            db.LandingPageImages.InsertOnSubmit(image);
            db.SubmitChanges();
            return GetImages(pageID);
        }

        public List<LandingPageImage> RemoveImage(int id) {
            CurtDevDataContext db = new CurtDevDataContext();
            int pageID = 0;
            try {
                LandingPageImage image = db.LandingPageImages.Where(x => x.id.Equals(id)).First();
                pageID = image.landingPageID;
                db.LandingPageImages.DeleteOnSubmit(image);
                db.SubmitChanges();

                List<LandingPageImage> images = db.LandingPageImages.Where(x => x.landingPageID.Equals(pageID)).OrderBy(x => x.sort).ToList();
                int sort = 1;
                foreach (LandingPageImage img in images) {
                    img.sort = sort++;
                }
                db.SubmitChanges();
            } catch { }
            return GetImages(pageID);
        }

        public void UpdateSort(int[] img) {
            CurtDevDataContext db = new CurtDevDataContext();
            int sort = 1;
            foreach (int imgID in img) {
                LandingPageImage image = db.LandingPageImages.Where(x => x.id.Equals(imgID)).First();
                image.sort = sort++;
                db.SubmitChanges();
            }
        }

        public List<LandingPageData> GetData(int pageID) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<LandingPageData> datas = new List<LandingPageData>();
            try {
                datas = db.LandingPageDatas.Where(x => x.landingPageID.Equals(pageID)).ToList();
            } catch { }
            return datas;
        }

        public List<LandingPageData> AddData(int pageID, string key, string value) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                LandingPageData data = new LandingPageData {
                    dataKey = key.Trim(),
                    dataValue = value.Trim(),
                    landingPageID = pageID
                };
                db.LandingPageDatas.InsertOnSubmit(data);
                db.SubmitChanges();
            } catch { }

            return GetData(pageID);
        }

        public List<LandingPageData> RemoveData(int id) {
            CurtDevDataContext db = new CurtDevDataContext();
            int pageID = 0;
            try {
                LandingPageData data = db.LandingPageDatas.Where(x => x.id.Equals(id)).First();
                pageID = data.landingPageID;
                db.LandingPageDatas.DeleteOnSubmit(data);
                db.SubmitChanges();
            } catch { }

            return GetData(pageID);
        }
    }
}