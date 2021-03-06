﻿/*
 * Author       : Alex Ninneman
 * Created      : January 20, 2011
 * Description  : This controller holds all of the page/AJAX functions for editing with vehicles in the CURT system.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using CurtAdmin.Models;
using Newtonsoft.Json;

namespace CurtAdmin.Controllers {
    public class ACESController : BaseController {

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            base.OnActionExecuted(filterContext);
            ViewBag.activeModule = "ACES Vehicle Data";
        }

        public ActionResult Vehicles() {
            List<vcdb_Make> makes = new ACES().GetMakes();
            ViewBag.makes = makes;
            return View();
        }

        public ActionResult AddVehicle() {
            List<AAIA.Make> makes = new ACES().GetVCDBMakes();
            ViewBag.makes = makes;

            List<int> years = new ACES().GetYears();
            ViewBag.years = years;

            List<ACESMake> allmakes = new ACES().GetAllMakes();
            ViewBag.allmakes = allmakes;

            List<ACESMake> allmodels = new ACES().GetAllModels();
            ViewBag.allmodels = allmodels;

            List<ACESMake> allsubmodels = new ACES().GetAllSubmodels();
            ViewBag.allsubmodels = allsubmodels;

            return View();
        }

        public string AddNonVCDBVehicle() {
            int year = Convert.ToInt32(Request.Form["nonyear"]);
            string make = Request.Form["nonmake"];
            string model = Request.Form["nonmodel"];
            string submodel = Request.Form["nonsubmodel"] ?? "";
            vcdb_Vehicle vehicle = new vcdb_Vehicle();
            try {
                vehicle = new ACES().AddNonVCDBVehicle(year, make, model, submodel);
            } catch { };
            return JsonConvert.SerializeObject(vehicle);
        }

        public string GetYears() {
            List<int> years = new ACES().GetYears();
            return JsonConvert.SerializeObject(years);
        }

        public string AddYear(int year) {
            vcdb_Year y = new ACES().AddYear(year);
            return JsonConvert.SerializeObject(y);
        }

        public string RemoveYear(int year) {
            try {
                new ACES().RemoveYear(year);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string GetAllMakes() {
            List<ACESMake> makes = new ACES().GetAllMakes();
            return JsonConvert.SerializeObject(makes);
        }

        public string AddMake(string make) {
            vcdb_Make m = new ACES().AddMake(make);
            return JsonConvert.SerializeObject(m);
        }

        public string UpdateMake(int id, string name) {
            vcdb_Make m = new ACES().UpdateMake(id,name);
            return JsonConvert.SerializeObject(m);
        }

        public string RemoveMake(string make) {
            try {
                new ACES().RemoveMake(make);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string GetAllModels() {
            List<ACESMake> makes = new ACES().GetAllModels();
            return JsonConvert.SerializeObject(makes);
        }

        public string GetVCDBModels(int id = 0) {
            List<AAIA.Model> models = new ACES().GetVCDBModels(id);
            return JsonConvert.SerializeObject(models);
        }

        public string AddModel(string model) {
            vcdb_Model m = new ACES().AddModel(Uri.UnescapeDataString(model));
            return JsonConvert.SerializeObject(m);
        }

        public string UpdateModel(int id, string name) {
            vcdb_Model m = new ACES().UpdateModel(id, Uri.UnescapeDataString(name));
            return JsonConvert.SerializeObject(m);
        }

        public string RemoveModel(string model) {
            try {
                new ACES().RemoveModel(model);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string GetAllSubmodels() {
            List<ACESMake> submodels = new ACES().GetAllSubmodels();
            return JsonConvert.SerializeObject(submodels);
        }

        public string AddSubmodelByName(string submodel) {
            Submodel s = new ACES().AddSubmodel(Uri.UnescapeDataString(submodel));
            return JsonConvert.SerializeObject(s);
        }

        public string UpdateSubmodel(int id, string name) {
            Submodel s = new ACES().UpdateSubmodel(id, Uri.UnescapeDataString(name));
            return JsonConvert.SerializeObject(s);
        }

        public string RemoveSubmodelByIDList(string submodel) {
            try {
                new ACES().RemoveSubmodel(submodel);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string GetBaseVehicles(int makeid, int modelid) {
            List<AAIA.BaseVehicle> basevehicles = new ACES().GetBaseVehicles(makeid, modelid);
            return JsonConvert.SerializeObject(basevehicles);
        }

        /*Aces Type methods */

        public ActionResult AcesTypes() {
            List<AcesType> types = new ACES().GetACESTypes();
            ViewBag.types = types;
            return View();
        }

        public ActionResult AddACESType(string error = "") {
            ViewBag.error = error;
            return View("EditACESType");
        }

        public ActionResult EditACESType(int id = 0, string error = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            AcesType type = new ACES().GetACESType(id);
            ViewBag.type = type;
            ViewBag.error = error;
            return View();
        }

        public ActionResult SaveACESType(int id = 0, string name = null) {
            CurtDevDataContext db = new CurtDevDataContext();
            AcesType type = new AcesType();
            string error = "";
            try {
                type = new ACES().SaveACESType(id, name);
            } catch (Exception e) {
                error = e.Message;
                if (id == 0) {
                    return RedirectToAction("AddACESType", new { error = error });
                } else {
                    return RedirectToAction("EditACESType", new { id = id, error = error });
                }
            }
            return RedirectToAction("AcesTypes");
        }

        public ActionResult RemoveACESType(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                AcesType t = db.AcesTypes.Where(x => x.ID.Equals(id)).First<AcesType>();
                db.AcesTypes.DeleteOnSubmit(t);
                db.SubmitChanges();
            } catch { }
            return RedirectToAction("AcesTypes");
        }

        /* Configuration Type Methods */

        public ActionResult ConfigTypes() {
            List<ConfigAttributeType> types = new ACES().GetConfigTypes();
            ViewBag.types = types;
            return View();
        }

        public ActionResult AddConfigurationType(string error = "") {
            ViewBag.error = error;
            List<AcesType> acestypes = new ACES().GetACESTypes();
            ViewBag.acestypes = acestypes;

            return View("EditConfigurationType");
        }

        public ActionResult EditConfigurationType(int id = 0, string error = "") {
            ACES aces = new ACES();
            ConfigAttributeType type = aces.GetConfigType(id);
            ViewBag.type = type;

            List<AcesType> acestypes = aces.GetACESTypes();
            ViewBag.acestypes = acestypes;
            
            ViewBag.error = error;
            return View();
        }
        
        public ActionResult SaveConfigurationType(int id = 0, string name = null, int? acestypeid = null) {
            ConfigAttributeType type = new ConfigAttributeType();
            string error = "";
            try {
                type = new ACES().SaveConfigurationType(id, name, acestypeid);
            } catch (Exception e) {
                error = e.Message;
                if (id == 0) {
                    return RedirectToAction("AddConfigurationType", new { error = error });
                } else {
                    return RedirectToAction("EditConfigurationType", new { id = id, error = error });
                }
            }
            return RedirectToAction("ConfigTypes");
        }

        public ActionResult RemoveConfigurationType(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                if (db.ConfigAttributes.Where(x => x.ConfigAttributeTypeID.Equals(id)).Count() == 0) {
                    ConfigAttributeType t = db.ConfigAttributeTypes.Where(x => x.ID.Equals(id)).First<ConfigAttributeType>();
                    db.ConfigAttributeTypes.DeleteOnSubmit(t);
                    db.SubmitChanges();
                }
            } catch { }
            return RedirectToAction("ConfigTypes");
        }

        /* Configuration Attribute Methods */

        public ActionResult ConfigAttributes() {
            List<ConfigAttribute> attributes = new ACES().GetConfigAttributes();
            ViewBag.attributes = attributes;
            return View();
        }

        public ActionResult AddConfigurationAttribute(string error = "") {
            List<ConfigAttributeType> configtypes = new ACES().GetConfigTypes();
            ViewBag.configtypes = configtypes;
            ViewBag.error = error;
            return View("EditConfigurationAttribute");
        }

        public ActionResult EditConfigurationAttribute(int id = 0, string error = "") {
            ACES aces = new ACES();

            ConfigAttribute attribute = aces.GetConfigAttribute(id);
            ViewBag.attribute = attribute;

            List<ConfigAttributeType> configtypes = aces.GetConfigTypes();
            ViewBag.configtypes = configtypes;
            ViewBag.error = error;
    
            return View();
        }

        public ActionResult SaveConfigurationAttribute(int id = 0, string value = null, int configtypeid = 0) {
            ConfigAttribute attribute = new ConfigAttribute();
            string error = "";
            try {
                attribute = new ACES().SaveConfigurationAttr(id, value, configtypeid);
            } catch (Exception e) {
                error = e.Message;
                if (id == 0) {
                    return RedirectToAction("AddConfigurationAttribute", new { error = error });
                } else {
                    return RedirectToAction("EditConfigurationAttribute", new { id = id, error = error });
                }
            }
            return RedirectToAction("ConfigAttributes");
        }

        public ActionResult RemoveConfigurationAttribute(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                if (db.vcdb_Vehicles.Where(x => x.VehicleConfig.VehicleConfigAttributes.Any(y => y.AttributeID.Equals(id))).Count() == 0) {
                    ConfigAttribute attr = db.ConfigAttributes.Where(x => x.ID.Equals(id)).First<ConfigAttribute>();
                    db.ConfigAttributes.DeleteOnSubmit(attr);
                    db.SubmitChanges();
                }
            } catch { }
            return RedirectToAction("ConfigAttributes");
        }

        public string GetModels(int id) {
            List<vcdb_Model> models = new ACES().GetModels(id);
            return JsonConvert.SerializeObject(models);
        }

        public string GetVehicles(int makeid, int modelid, int partID = 0) {
            List<ACESBaseVehicle> vehicles = new List<ACESBaseVehicle>();
            vehicles = new ACES().GetVehicles(makeid, modelid, partID);
            return JsonConvert.SerializeObject(vehicles);
        }

        public string GetVCDBVehicles(int makeid, int modelid) {
            List<VCDBBaseVehicle> vehicles = new List<VCDBBaseVehicle>();
            vehicles = new ACES().GetVCDBVehicles(makeid, modelid);
            return JsonConvert.SerializeObject(vehicles);
        }

        public string AddBaseVehicle(int id) {
            vcdb_Vehicle vehicle = new vcdb_Vehicle();
            vehicle = new ACES().AddBaseVehicle(id);
            return JsonConvert.SerializeObject(vehicle);
        }

        public string RemoveBaseVehicle(int id) {
            try {
                new ACES().RemoveBaseVehicle(id);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string AddSubmodel(int BaseVehicleID, int SubmodelID) {
            vcdb_Vehicle vehicle = new vcdb_Vehicle();
            vehicle = new ACES().AddVCDBSubmodel(BaseVehicleID, SubmodelID);
            return JsonConvert.SerializeObject(vehicle);
        }

        public string RemoveSubmodel(int BaseVehicleID, int SubmodelID) {
            try {
                new ACES().RemoveSubmodel(BaseVehicleID, SubmodelID);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string GetConfigs(int BaseVehicleID, int SubmodelID) {
            ACESVehicleOptions configs = new ACESVehicleOptions();
            configs = new ACES().getVehicleConfigs(BaseVehicleID, SubmodelID);
            return JsonConvert.SerializeObject(configs);
        }

        public string GetConfigsByVehicle(int id) {
            ACESVehicleOptions configs = new ACESVehicleOptions();
            configs = new ACES().getVehicleConfigs(id);
            return JsonConvert.SerializeObject(configs);
        }

        public string checkVehicle(int id) {
            vcdb_Vehicle vehicle = new vcdb_Vehicle();
            vehicle = new ACES().GetVehicle(id);
            return JsonConvert.SerializeObject(vehicle);
        }

        public string checkVehicleExists(int vehicleID, int attributeID, string method = "remove") {
            int duplicateID = new ACES().checkVehicleExists(vehicleID, attributeID, method);
            return JsonConvert.SerializeObject(duplicateID);
        }

        public string checkVehicleAndAttributeExists(int vehicleID, int attributeID) {
            int duplicateID = new ACES().checkVehicleExists(vehicleID, attributeID);
            return JsonConvert.SerializeObject(duplicateID);
        }

        public string checkVehicleAndNewAttributeExists(int vehicleID, int vcdbID, int typeID, string textvalue) {
            int duplicateID = new ACES().checkVehicleExists(vehicleID, vcdbID, typeID, textvalue);
            return JsonConvert.SerializeObject(duplicateID);
        }

        public string mergeVehicles(int targetID, int currentID, bool deleteCurrent = true) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().mergeVehicles(targetID, currentID, deleteCurrent);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string addAttributeToVehicle(int vehicleID, int vcdbID, int typeID, string value) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().addAttributeToVehicle(vehicleID, vcdbID, typeID, value);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string addAttribute(int vehicleID, int vcdbID, int typeID, string value) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().addAttribute(vehicleID, vcdbID, typeID, value);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string removeAttribute(int vehicleID, int attributeID) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().removeAttribute(vehicleID, attributeID);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string addConfig(int BaseVehicleID, int SubmodelID, string configs = "") {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            List<int> configids = new List<int>();
            if (configs != "") {
                configids = configs.Split(',').Select(s => int.Parse(s)).ToList();
                basevehicle = new ACES().addConfig(BaseVehicleID, SubmodelID, configids);
            } else {
                new ACES().AddSubmodel(BaseVehicleID, SubmodelID);
                basevehicle = new ACES().GetVehicle(BaseVehicleID,SubmodelID);
            }
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string removeVehicle(int id) {
            try {
                new ACES().RemoveVehicle(id);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string getNonACESConfigurationTypes(int id) {
            List<ConfigAttributeType> types = new List<ConfigAttributeType>();
            types = new ACES().getNonACESConfigurationTypes(id);
            return JsonConvert.SerializeObject(types);
        }

        public ActionResult RunVehicleCheck(int? updatedVehicles = null) {
            ViewBag.updatedVehicles = updatedVehicles;
            return View();
        }

        public ActionResult RunCheck() {
            int vehicles = new ACES().RunCheck();
            return RedirectToAction("RunVehicleCheck", new { updatedVehicles = vehicles });
        }

        public string GetAttributesByType(int id) {
            List<ConfigAttribute> attributes = new List<ConfigAttribute>();
            attributes = new ACES().GetAttributesByType(id);
            return JsonConvert.SerializeObject(attributes);
        }

        public string AddCustomConfigToVehicle(int vehicleID, int attrID) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().addCustomConfigToVehicle(vehicleID, attrID);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string AddCustomConfig(int vehicleID, int attrID) {
            ACESBaseVehicle basevehicle = new ACESBaseVehicle();
            basevehicle = new ACES().addCustomConfig(vehicleID, attrID);
            return JsonConvert.SerializeObject(basevehicle);
        }

        public string SearchPartTypes(string keyword = "") {
            return new ACES().SearchPartTypes(keyword);
        }

        public string GetPartTypeByID(int id = 0) {
            return new ACES().GetPartTypeByID(id);
        }

        public string GetVehicleParts(int vehicleID, int baseVehicleID, int submodelID) {
            List<vcdb_VehiclePart> parts = new List<vcdb_VehiclePart>();
            try {
                if (vehicleID != 0) {
                    parts = new ACES().GetVehicleParts(vehicleID);
                } else {
                    parts = new ACES().GetVehicleParts(baseVehicleID, submodelID);
                }
            } catch { }
            return JsonConvert.SerializeObject(parts);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string RemoveVehiclePart(int id) {
            try {
                new ACES().RemoveVehiclePart(id);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string AddVehiclePart(int vehicleID = 0, int baseVehicleID = 0, int submodelID = 0, int partID = 0, string partOrVehicle = "vehicle") {
            List<vcdb_VehiclePart> vparts = new List<vcdb_VehiclePart>();
            try {
                vparts = new ACES().AddVehiclePart(vehicleID, baseVehicleID, submodelID, partID, partOrVehicle);
            } catch { }
            return JsonConvert.SerializeObject(vparts);
        }

        public string GetNotes(int id) {
            List<Note> notes = new List<Note>();
            try {
                notes = new ACES().getNotes(id);
            } catch { }
            return JsonConvert.SerializeObject(notes);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string RemoveNote(int id) {
            try {
                new ACES().RemoveNote(id);
                return "{\"success\":true}";
            } catch {
                return "{\"success\":false}";
            }
        }

        public string AddNote(int vPartID, string note) {
            try {
                new ACES().AddNote(vPartID, note);
            } catch { }
            return GetNotes(vPartID);
        }

        public string SearchNotes(string keyword = "") {
            return new ACES().SearchNotes(keyword);
        }

        public string SearchMakes(string keyword = "") {
            return new ACES().SearchMakes(keyword);
        }

        public string SearchModels(string keyword = "") {
            return new ACES().SearchModels(keyword);
        }

        public string SearchSubmodels(string keyword = "") {
            return new ACES().SearchSubmodels(keyword);
        }

        public string PopulatePartsFromBaseVehicle(int vehicleID = 0, int baseVehicleID = 0, int submodelID = 0) {
            try {
                new ACES().PopulatePartsFromBaseVehicle(vehicleID, baseVehicleID, submodelID);
            } catch { }
            return GetVehicleParts(vehicleID, baseVehicleID, submodelID);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string MapPart(int id) {
            List<Vehicles> unmapped = new ACES().MapPart(id);
            return JsonConvert.SerializeObject(unmapped);
        }

    }

    
}
