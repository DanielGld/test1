using System;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    public class PhotosManagement
    {
        public Photo[] PicsInAlbum { get; private set; }

        public Album MobileUploadsAlbum { get; private set; }
        
        public int TotalNumOfPhotos { get; private set; } = 0;

        public PhotosManagement(User i_User)
        {
            getAlbum(i_User);
            getAllPhotos(i_User);
        }

        public void SortPhotosBy(string i_SortBy)
        {
            int[] sortedPhotos = new int[TotalNumOfPhotos];
            int[] originalArry = new int[TotalNumOfPhotos];

            for (int i = 0; i < TotalNumOfPhotos; i++)
            {
                if (i_SortBy.ToLower() == "most comments")
                {
                    sortedPhotos[i] = MobileUploadsAlbum.Photos[i].Comments.Count;
                    originalArry[i] = sortedPhotos[i];
                }
                else if (i_SortBy.ToLower() == "most likes")
                {
                    sortedPhotos[i] = MobileUploadsAlbum.Photos[i].LikedBy.Count;
                    originalArry[i] = sortedPhotos[i];
                }
                else ////most recent
                {
                    break;
                }
            }

            Array.Sort(sortedPhotos);
            for (int i = TotalNumOfPhotos - 1; i >= 0; i--)
            {
                for (int j = 0; j < TotalNumOfPhotos; j++)
                {
                    if (sortedPhotos[i] == originalArry[j])
                    {
                        PicsInAlbum[TotalNumOfPhotos - 1 - i] = MobileUploadsAlbum.Photos[j];
                        originalArry[j] = -1;
                        break;
                    }
                }
            }
        }

        private void getAlbum(User i_User)
        {
            foreach (Album album in i_User.Albums)
            {
                if (album.Name.ToLower() == "mobile uploads")
                {
                    MobileUploadsAlbum = album;
                }
            }
        }

        private void getAllPhotos(User i_User)
        {
            try
            {
                TotalNumOfPhotos = MobileUploadsAlbum.Photos.Count;
                PicsInAlbum = new Photo[TotalNumOfPhotos];
                for (int i = 0; i < TotalNumOfPhotos; i++)
                {
                    PicsInAlbum[i] = MobileUploadsAlbum.Photos[i];
                }
            }
            catch{ }
        }
    }
}