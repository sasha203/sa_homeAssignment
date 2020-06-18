using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using BusinessLogic;
using System.IO;
using System.Text;


namespace SA_securityAssignment.Controllers
{
    [Authorize]
    public class TracksController : Controller
    {

        public ActionResult Index() 
        {
            TracksBL tbl = new TracksBL();
            IList<Track> tracks = tbl.GetTracks().ToList();
            return View(tracks);
        }


        public ActionResult Create() 
        {
            return View();
        }


        [HttpPost] //On submit, will pass data through post method not from URL.
        [ValidateAntiForgeryToken] //CSFT (Cross Site Request Forgery)
        public ActionResult Create(Track t, HttpPostedFileBase fileData) 
        {
            try
            {
                if (fileData != null)
                {

                    if (t.Title == null) {
                        throw new CustomException("Please enter track title.");
                    }


                    if (ModelState.IsValid)
                    {
                        //if file is >= 8MB (in bytes) will throw exception.
                        if (fileData.ContentLength >= 8388608)
                        {
                            throw new CustomException("File must be less than 8MB");
                        }


                        //https://www.garykessler.net/library/file_sigs.html
                        //only mp3s & wav


                        //byte[] mp3Buff1 = new byte[10];
                        // fileData.InputStream.Read(mp3Buff1, 0, 10);


                        string uniqueFilename = Guid.NewGuid() + Path.GetExtension(fileData.FileName); //creating a unique filename for the track URL.

                        //absolute path the following:  I:\Securing Applications assignment\SecuringApplications\SecurityWebsite\Tracks
                        string absolutePath = Server.MapPath(@"\TrackStorage") + @"\"; //@ is used as an esacpe character.
                        

                        //l path li se jigi saved go server.
                        t.TrackUrl = @"\TrackStorage\" + uniqueFilename; //Relative path.


                        UsersBL ub = new UsersBL();
                        User currentUser = ub.GetUser(User.Identity.Name);


                        MemoryStream encryptedFile = Encryption.HybridEncrypt(fileData.InputStream, currentUser.PublicKey);

                        MemoryStream fd = new MemoryStream();
                        encryptedFile.Position = 0;
                        encryptedFile.CopyTo(fd);


                        Encryption e = new Encryption();
                        string signature = e.SignData(fd.ToArray(), currentUser.PrivateKey);

                        t.digitalSignature = signature;

                        //Creating the encrypted file in TrackStorage.
                        System.IO.File.WriteAllBytes(absolutePath + uniqueFilename, encryptedFile.ToArray());


                        new TracksBL().AddTrack(t, currentUser);
                        TempData["message"] = "Track Successfully Added";
                        Logger.Log(currentUser.Username, Request.Path, "Track " + t.Title + " Was Successfully Added.");

                        return RedirectToAction("Index");
                    }
                }
                else {
                    throw new CustomException("Please upload an audio track.");
                }

                return View(t);

            }
            catch (CustomException ex) {
                TempData["errormessage"] = ex.Message;
                Logger.Log(User.Identity.Name, Request.Path, ex.Message);

                return View(t);
            }

            catch (Exception ex)
            {
                TempData["errormessage"] = ex.Message;
                Logger.Log(User.Identity.Name, Request.Path, ex.Message);
                return View(t);
            }
        }



        public ActionResult Details(string id)
        {
            try
            {
                TracksBL tbl = new TracksBL();
                string decryptedId = Encryption.DecryptQueryString(id);

                Track t = tbl.GetTrack(Convert.ToInt32(decryptedId));
                return View(t);
            }
            catch (Exception e) {
                Logger.Log(User.Identity.Name, Request.Path, "Access Denied!", e.Message);
                TempData["errormessage"] = "Access Denied!";
                return RedirectToAction("Index");
            }
        }


        [Authorize]
        public ActionResult Download(int id)
        {

            try
            {
                //check if user has permission to download the file (Not yet done)


                //Add admin role task5
                Track track = new TracksBL().GetTrack(id);
                User currentUser = new UsersBL().GetUser(User.Identity.Name);

                if (track.TrackUrl != null)
                {

                    string absolutePath = Server.MapPath(track.TrackUrl);

                    if (System.IO.File.Exists(absolutePath))
                    {

                        byte[] data = System.IO.File.ReadAllBytes(absolutePath); //track as an array of bytes

                        //Decryption of (hybrid encryption)
                        MemoryStream msIn = new MemoryStream(data); //track as a memoryStream
                        msIn.Position = 0;



                        //Digital signing verifying
                        
                        Encryption e = new Encryption();
                        User trackOwner = new UsersBL().GetUserById(track.userId);

                        //true if not altered, false otherwise.
                        //params : the file, public key of who uploaded the track, the track ds
                        bool secureTrack = e.VerifyData(msIn.ToArray(), trackOwner.PublicKey, track.digitalSignature);

                        if (secureTrack)
                        {
                            MemoryStream msDecrypted = Encryption.HybridDecrypt(msIn, currentUser.PrivateKey);

                            Logger.Log(User.Identity.Name, Request.Path, "Downloaded " + track.Title);

                            return File(msDecrypted.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
                                Path.GetFileName(track.TrackUrl));
                        }
                        else {  
                            throw new CustomException("Download failed: File compromised");
                        }
                    }

                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                Logger.Log(User.Identity.Name, Request.Path, "Error: " + ex.Message);
                TempData["errormessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {

            try
            {
                Track t = new TracksBL().GetTrack(id);


                string absolutePath = Server.MapPath(t.TrackUrl);

                if (System.IO.File.Exists(absolutePath))
                {
                    //namel admin role awek habba Task5
                    new TracksBL().RemoveTrack(id);

                    System.IO.File.Delete(absolutePath);

                    Logger.Log(User.Identity.Name, Request.Path, "Track " + t.Title + " was deleted");
                    TempData["message"] = "Track deleted successfully";
                }
                   

            }
            catch (Exception ex)
            {
                Logger.Log(User.Identity.Name, Request.Path, "Error: " + ex.Message);
                TempData["errormessage"] = "Track was not deleted";
            }

            return RedirectToAction("Index");
        }


    }
}