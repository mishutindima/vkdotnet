using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using ApiCore;
using ApiCore.Friends;
using ApiCore.Photos;
using ApiCore.Utils.SessionManager;
using ApiCore.WinForms;

namespace PhotoExplorer
{
    public partial class MainWnd : Form
    {
        private string appTitle = "Photo Explorer";
        private SessionInfo sessionInfo;
        private ApiManager manager;

        private bool isLoggedIn = false;
        private bool downloadInProgress = false;
        private bool exitPending = false;
        private bool userIdIsIncorrect = true;

        private int friendItemsCount = 200;
        private int friendCurrentOffset = 0;
        private int itemsToDownload = 0;
        private int photosInAlbum = 0;
        private int itemsDownloaded = 1;

        private Friend selectedFriend;
        private AlbumEntry selectedAlbum;

        private List<Friend> friendList;
        private List<AlbumEntry> albumList;
        private List<PhotoEntryFull> photoList;

        private FriendsFactory friendFactory;
        private PhotosFactory photoFactory;

        private Regex userIdCheck;


        public MainWnd()
        {
            InitializeComponent();
        }

        private void MainWnd_Resize(object sender, EventArgs e)
        {
            this.DownloadPanel.Top = this.Height / 2 - this.DownloadPanel.Height / 2;
            this.DownloadPanel.Left = this.Width / 2 - this.DownloadPanel.Width / 2;
        }

        private void MainWnd_Shown(object sender, EventArgs e)
        {
            this.Text = this.appTitle + ": Not Authorized!";
            this.Reauth();
            this.GetFriendList();
        }

        private void reloadAudioListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.AlbumsList.Items.Clear();
            this.albumList.Clear();
            this.GetFriendList();
        }

        private void FriendList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedFriend = this.friendList[((ListBox)sender).SelectedIndex];
            this.LoadUserAlbums(this.selectedFriend.Id);
        }

        private void UserId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (UserId.Text != "" && this.userIdIsIncorrect == false)
                {
                    this.LoadUserAlbums(Convert.ToInt32(this.userIdCheck.Match(UserId.Text).Value));
                }
                else
                {
                    MessageBox.Show("Entered user id is incorrect!");
                }
            }
        }

        private void UserId_TextChanged(object sender, EventArgs e)
        {
            if(this.userIdCheck.IsMatch(((ToolStripTextBox)sender).Text))
            {
                 ((ToolStripTextBox)sender).BackColor = SystemColors.Window;
                 this.userIdIsIncorrect = false;
            }
            else
            {
                ((ToolStripTextBox)sender).BackColor = Color.LightCoral;
                this.userIdIsIncorrect = true;
            }
        }

        private void reauthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Reauth();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowAboutBox();
        }

        private void Reauth()
        {
            if (!this.isLoggedIn)
            {
                SessionManager sm = new SessionManager(1928531, Convert.ToInt32(ApiPerms.Audio | ApiPerms.ExtendedMessages | ApiPerms.ExtendedWall | ApiPerms.Friends | ApiPerms.Offers | ApiPerms.Photos | ApiPerms.Questions | ApiPerms.SendNotify | ApiPerms.SidebarLink | ApiPerms.UserNotes | ApiPerms.UserStatus | ApiPerms.Video | ApiPerms.WallPublisher | ApiPerms.Wiki));
                sm.OAuthRequired += sm_OAuthRequired;
                this.sessionInfo = sm.GetOAuthSession(false);
                if (this.sessionInfo != null)
                {
                    this.isLoggedIn = true;
                }
            }

            if (this.isLoggedIn)
            {
                manager = new ApiManager(this.sessionInfo);
                manager.OnCapthaRequired += manager_OnCapthaRequired;
                //manager.Log += new ApiManagerLogHandler(manager_Log);
                //manager.DebugMode = true;
                manager.Timeout = 10000;
                this.FriendList.Enabled = true;
                this.Text = this.appTitle + ": Authorization success!";

                this.friendFactory = new FriendsFactory(this.manager);
                this.photoFactory = new PhotosFactory(this.manager);
                this.photoList = new List<PhotoEntryFull>();
                this.albumList = new List<AlbumEntry>();
                this.userIdCheck = new Regex("([\\d])+$");
                this.GetFriendList();
            }

        }

        void sm_OAuthRequired(object sender, OAuthEventArgs e)
        {
            var wnd = new OAuthWnd(e.Uries);
            wnd.ShowDialog();
            e.TokenResponse = wnd.TokenResponse;
        }

        void manager_OnCapthaRequired(object sender, string url, string hash)
        {
            var wnd = new CapthaWnd(manager, url, hash);
            wnd.ShowDialog();
        }

        private void GetFriendList()
        {
            try
            {
                this.AlbumsList.Enabled = false;
                this.FriendList.Items.Clear();
                this.friendList = this.friendFactory.Get( Convert.ToInt32(UserId.Text), FriendNameCase.Nominative, null, null, null, new string[] { "uid", "first_name", "nickname", "last_name" });
                for (int i = 0; i < this.friendList.Count; i++)
                {
                    this.FriendList.Items.Add(this.friendList[i]);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Error loading friend list:\n"+e.Message);
            }
        }

        private void LoadUserAlbums(int friendId)
        {
            try
            {
                this.AlbumsList.Items.Clear();
                this.albumList = this.photoFactory.GetAlbums(friendId, null);
                for (int i = 0; i < this.albumList.Count; i++)
                {
                    this.AlbumsList.Items.Add(this.albumList[i]);
                }
                this.AlbumsList.Enabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading friend albums:\n" + e.Message);
            }
        }

        private void ShowAboutBox()
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://xternalx.com";
            p.Start();
        }
    }
}
