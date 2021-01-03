﻿using System;
using System.Threading;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using Logic;

namespace FaceBookUI
{
    public partial class MainPageForm : Form
    {
        readonly int r_TotalNumOfDisplyedPhotos = 8;
        int m_NumOfPhotosToShow = 0;
        User m_LoggedInUser = null;
        FbBasicOpertion m_AppStart;
        PhotosManagement m_PhotosManagement;
        PostsManagement m_PostsManagement;
        PictureBoxProxy[] m_PictureBoxsArr;
        PersonalDetailsForm m_PersonalInfo;
        FriendshipTestForm m_FriendshipTest;
        Map m_PhotoLocationMap;
        AppSettings m_AppSettings = new AppSettings();

        public MainPageForm()
        {
            InitializeComponent();
            makeArryOfPictureBoxes();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            loginToFacebook();
        }

        private void loginToFacebook()
        {
            m_AppStart = FbBasicOpertion.Instance;
            if (buttonLogin.Text == "Login")
            {
                m_AppSettings.LoadFromFile();
                checkBoxRememberMe.Checked = m_AppSettings.RememberMe;
                try
                {
                    m_LoggedInUser = m_AppStart.LoginToFb(m_AppSettings.LastAccessToken);
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not connect to FaceBook \n Please try again later.");
                    System.Environment.Exit(0);
                }

                m_PersonalInfo = new PersonalDetailsForm(m_LoggedInUser);
                m_PhotosManagement = new PhotosManagement(m_LoggedInUser);
                m_PostsManagement = new PostsManagement();
                m_PhotoLocationMap = new Map();
                allControlStatus(true);
                buttonPreviousPhotos.Enabled = false;
                buttonLogin.Text = "LogOut";
                displayUserDetails();
            }
            else // log out
            {
                buttonLogin.Text = "Login";
                saveSettings();
                m_AppStart.LogOutFromFB(); 
                clearAllFields();
            }
        }

        private void displayUserDetails()
        {
            new Thread(addPictures).Start();
            new Thread(addFriendsToList).Start();
            new Thread(addWallpostsToList).Start();
            displayPhotosSets(0);
        }

        private void addPictures()
        {
            pictureProfile.LoadAsync(m_LoggedInUser.PictureNormalURL);
            foreach (Album album in m_LoggedInUser.Albums)
            {
                if (album.Name.ToLower() == "cover photos")
                {
                    pictureBoxCoverPic.LoadAsync(album.Photos[0].PictureNormalURL);
                }
            }
        }

        private void addFriendsToList()
        {
            listBoxFriends.Invoke(new Action(() => listBoxFriends.DisplayMember = "Name")); 
            foreach (User friend in m_LoggedInUser.Friends)
            {
                listBoxFriends.Invoke(new Action(() => listBoxFriends.Items.Add(friend)));
            }
        }

        private void addWallpostsToList()
        {
            Invoke(new Action(() => postBindingSource.DataSource = m_LoggedInUser.WallPosts));
        }

        private void allControlStatus(bool value)
        {
            foreach (Control control in this.Controls)
            {
                control.Enabled = value;
            }

            buttonLogin.Enabled = true;
        }

        private void clearAllFields()
        {
            listBoxFriends.Items.Clear();
            postBindingSource.Clear();
            allControlStatus(false);
            foreach (Control control in Controls)
            {
                if (control is PictureBox)
                {
                    ((PictureBox)control).ImageLocation = null;
                }
            }
        }

        private void displayPhotosSets(int i_PicNum = 0)
        {
            try
            {
                for (int i = 0; i < m_PictureBoxsArr.Length; i++)
                {
                    if (i + i_PicNum < m_PhotosManagement.PicsInAlbum.Length)
                    {
                        m_PictureBoxsArr[i].LoadAsync(m_PhotosManagement.PicsInAlbum[i + i_PicNum]);
                    }
                    else
                    {
                        m_PictureBoxsArr[i].Image = null;
                    }
                }
            }
            catch{ }
        }

        private void makeArryOfPictureBoxes()
        {
            m_PictureBoxsArr = new PictureBoxProxy[r_TotalNumOfDisplyedPhotos];
            m_PictureBoxsArr[0] = pictureBoxProxy1;
            m_PictureBoxsArr[1] = pictureBoxProxy2;
            m_PictureBoxsArr[2] = pictureBoxProxy3;
            m_PictureBoxsArr[3] = pictureBoxProxy4;
            m_PictureBoxsArr[4] = pictureBoxProxy5;
            m_PictureBoxsArr[5] = pictureBoxProxy6;
            m_PictureBoxsArr[6] = pictureBoxProxy7;
            m_PictureBoxsArr[7] = pictureBoxProxy8;
        }

        private void buttonNextPhotos_Click(object sender, EventArgs e)
        {
            m_NumOfPhotosToShow += r_TotalNumOfDisplyedPhotos;
            displayPhotosSets(m_NumOfPhotosToShow);
            if (m_NumOfPhotosToShow + r_TotalNumOfDisplyedPhotos >= m_PhotosManagement.PicsInAlbum.Length)
            {
                buttonNextPhotos.Enabled = false;
            }

            if (m_NumOfPhotosToShow - r_TotalNumOfDisplyedPhotos >= 0)
            {
                buttonPreviousPhotos.Enabled = true;
            }
        }

        private void buttonPreviousPhotos_Click(object sender, EventArgs e)
        {
            m_NumOfPhotosToShow -= r_TotalNumOfDisplyedPhotos;
            displayPhotosSets(m_NumOfPhotosToShow);
            if (m_NumOfPhotosToShow + r_TotalNumOfDisplyedPhotos <= m_PhotosManagement.PicsInAlbum.Length) 
            {
                buttonNextPhotos.Enabled = true;
            }

            if (m_NumOfPhotosToShow - r_TotalNumOfDisplyedPhotos < 0) 
            {
                buttonPreviousPhotos.Enabled = false;
            }
        }

        private void comboBoxSortPhotos_TextChanged(object sender, EventArgs e)
        {
            m_PhotosManagement.SortPhotosBy(comboBoxSortPhotos.Text);
            displayPhotosSets(0);
            m_NumOfPhotosToShow = 0;
            buttonNextPhotos.Enabled = true;
            buttonPreviousPhotos.Enabled = false;
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            if (textBoxPost.Text.Length < 1)
            {
                MessageBox.Show("Can't post empty");
            }
            else
            {
                try
                {
                    m_PostsManagement.MakePost(m_LoggedInUser, textBoxPost.Text);          
                }
                catch
                {
                    MessageBox.Show("Can't post due to facebook permissions");
                }
            }
        }

        private void personalInfo_Click(object sender, EventArgs e)
        {
            m_PersonalInfo.ShowDialog();
        }

        private void buttonFriendshipTest_Click(object sender, EventArgs e)
        {
            try
            {
                m_FriendshipTest = new FriendshipTestForm(listBoxFriends.SelectedItem as User);
                m_FriendshipTest.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Please choose a friend from the friend list");
            }
        }

        private void saveSettings()
        {
            try
            {
                if (checkBoxRememberMe.Checked == true)
                {
                    m_AppSettings.RememberMe = checkBoxRememberMe.Checked;
                    m_AppSettings.LastAccessToken = m_AppStart.LoginResult.AccessToken;
                    m_AppSettings.SaveToFile();
                }
                else
                {
                    m_AppSettings.RestoreDefault();
                    m_AppSettings.SaveToFile();
                }
            }
            catch
            {
                MessageBox.Show("An error occured while saving settings to file");
            }
        }

        private void mainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
        }

        private void showPictureLocation(PictureBoxProxy i_Photo)
        {
            {
                if (i_Photo.Photo.Place != null)
                {
                    double? latitude = i_Photo.Photo.Place.Location.Latitude;
                    double? longitude = i_Photo.Photo.Place.Location.Longitude;

                    if (latitude != null && longitude != null)
                    {
                        m_PhotoLocationMap.SetLocation((double)latitude, (double)longitude);
                        m_PhotoLocationMap.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("No place was defined in this photo");
                }
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            showPictureLocation(sender as PictureBoxProxy);
        }
    }
}