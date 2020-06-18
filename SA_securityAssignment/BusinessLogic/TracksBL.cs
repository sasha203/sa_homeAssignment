using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataAccess;

using System.IO;

namespace BusinessLogic
{
    public class TracksBL
    {
        public IQueryable<Track> GetTracks()
        {
            return new TracksRepository().GetTracks();
        }

        public Track GetTrack(int trackId) {
            return new TracksRepository().GetTrack(trackId);
        }
        
      
        public void AddTrack(Track t, User currentUser)
        {

            Track newTrack = new Track();
            newTrack.Title = t.Title;
            newTrack.GenreId = t.GenreId;
            newTrack.userId = currentUser.Id;
            newTrack.digitalSignature = t.digitalSignature;

            if (string.IsNullOrEmpty(t.TrackUrl) == false) //if not empty add track url
            {
                newTrack.TrackUrl = t.TrackUrl;
            }

          
            new TracksRepository().AddTrack(newTrack);
        }

        public void RemoveTrack(int trackId) {
            TracksRepository tr = new TracksRepository();
            Track t = tr.GetTrack(trackId);
            if (t != null)
            {
                tr.RemoveTrack(t);
            }
        }


    }
}
