using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Browser;

namespace SLFileUpload
{
    public partial class FileUploadControl : UserControl
    {
        public int MaxConcurrentUploads { get; set; }
        public long UploadChunkSize { get; set; }
        public bool ResizeImage { get; set; }
        public int ImageSize { get; set; }
        private DateTime start;

        public Brush BackgroundColor
        {
            get { return controlBorder.Background; }
            set { controlBorder.Background = value; }
        }
        public CornerRadius CornerRadius
        {
            get { return controlBorder.CornerRadius; }
            set { controlBorder.CornerRadius = value; }
        }
        public Brush BorderBrushColor
        {
            get { return controlBorder.BorderBrush; }
            set { controlBorder.BorderBrush = value; }
        }
        public Thickness BorderThickness
        {
            get { return controlBorder.BorderThickness; }
            set { controlBorder.BorderThickness = value; }
        }

        public string Filter { get; set; }
        public bool Multiselect { get; set; }
        public Uri UploadUrl { get; set; }
        public string FileType { get; set; }
        public string JavascriptCompleteFunction { get; set; }

        private bool uploading { get; set; }

        private long TotalUploadSize { get; set; }
        private long TotalUploaded { get; set; }

        public long MaximumTotalUpload { get; set; }
        public long MaximumUpload { get; set; }

        public int MaxNumberToUpload { get; set; }
        private int count = 0;

        public bool AllowThumbnail 
        {
            get { return displayThumbailChckBox.Visibility == Visibility.Visible; }
            set 
            {
                bool temp = value;
                if (temp) 
                    displayThumbailChckBox.Visibility = Visibility.Visible; 
                else 
                    displayThumbailChckBox.Visibility = Visibility.Collapsed; 
            }
        }

        private ScrollHelper helper;

        private ObservableCollection<FileUpload> files;


        public FileUploadControl()
        {
            InitializeComponent();
            files = new ObservableCollection<FileUpload>();
            files.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(files_CollectionChanged);
            MaxConcurrentUploads = 1;
            MaxNumberToUpload = -1;
            MaximumTotalUpload = MaximumUpload = -1;
            Filter = "Excel 文件（*.xls,*.xlsx）|*.xls;*.xlsx";
            Multiselect = true;
            uploading = false;
            UploadChunkSize = 4194304;

            addFilesButton.Click += new RoutedEventHandler(addFilesButton_Click);
            uploadButton.Click += new RoutedEventHandler(uploadButton_Click);
            clearFilesButton.Click += new RoutedEventHandler(clearFilesButton_Click);

            displayThumbailChckBox.Checked += new RoutedEventHandler(displayThumbailChckBox_Checked);
            displayThumbailChckBox.Unchecked += new RoutedEventHandler(displayThumbailChckBox_Checked);

            helper = new ScrollHelper(filesScrollViewer);

            Loaded += new RoutedEventHandler(FileUploadControl_Loaded);
        }

        void displayThumbailChckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = sender as CheckBox;

            foreach (FileUpload fu in files)
            {
                if (fu.DisplayThumbnail != chkbox.IsChecked)
                    fu.DisplayThumbnail = (bool)chkbox.IsChecked;
            }
        }
        public FileUploadControl(Uri uploadUrl)
            : this()
        {
            UploadUrl = uploadUrl;
        }

        void files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            countTextBlock.Text = "总数: " + files.Count.ToString();
            TotalUploadSize = files.Sum(f => f.FileLength);
            TotalUploaded = files.Sum(f => f.BytesUploaded);
            totalSizeTextBlock.Text = string.Format("{0} of {1}",
                new ByteConverter().Convert(TotalUploaded, this.GetType(), null, null).ToString(),
                new ByteConverter().Convert(TotalUploadSize, this.GetType(), null, null).ToString());
            progressBar.Maximum = TotalUploadSize;
            progressBar.Value = TotalUploaded;
        }

        void clearFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var q = files.Where(f => f.Status == FileUploadStatus.Uploading);
            foreach (FileUpload fu in q)
                fu.CancelUpload();
            timeLeftTextBlock.Text = "";
            files.Clear();
        }

        void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)uploadButton.Content == " 上 传 ")
            {
                uploadButton.Content = " 取 消 ";
                start = DateTime.Now;
                UploadFiles();
            }
            else
            {
                var q = files.Where(f => f.Status == FileUploadStatus.Uploading);
                foreach (FileUpload fu in q)
                    fu.CancelUpload();
                uploading = false;
                uploadButton.Content = " 上 传 ";
            }
        }

        void addFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = Filter;
            dlg.Multiselect = Multiselect;

            if ((bool)dlg.ShowDialog())
            {
                foreach (FileInfo file in dlg.Files)
                {
                    FileUpload upload;
                    try
                    {
                        upload = new FileUpload(this.Dispatcher, UploadUrl, file, FileType);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }
                        
                    if (UploadChunkSize > 0)
                        upload.ChunkSize = UploadChunkSize;
                    if (ResizeImage)
                    {
                        upload.ResizeImage = ResizeImage;
                        upload.ImageSize = ImageSize;
                    }

                    if (MaxNumberToUpload > -1)
                    {
                        count++;
                        if (count > MaxNumberToUpload)
                        {
                            MessageBox.Show("超过允许上传文件数量的上限！");
                            break;
                        }
                    }

                    if (MaximumTotalUpload >= 0 && TotalUploadSize + upload.FileLength > MaximumTotalUpload)
                    {
                        MessageBox.Show("文件总大小超出了允许的范围。");
                        break;
                    }
                    if (MaximumUpload >= 0 && upload.FileLength > MaximumUpload)
                    {
                        MessageBox.Show(string.Format("文件： '{0}' 大小超出了允许的范围。", upload.Name));
                        continue;
                    }
                    upload.DisplayThumbnail = (bool)displayThumbailChckBox.IsChecked;
                    upload.StatusChanged += new EventHandler(upload_StatusChanged);
                    upload.UploadProgressChanged += new ProgressChangedEvent(upload_UploadProgressChanged);
                    upload.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(upload_PropertyChanged);
                    files.Add(upload);
                }
            }
        }

        void upload_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {            
            if (e.PropertyName == "FileLength")
            {
                files_CollectionChanged(null, null);
            }
        }

        void upload_UploadProgressChanged(object sender, UploadProgressChangedEventArgs args)
        {
            

            TotalUploaded += args.BytesUploaded;
            progressBar.Value = TotalUploaded;
            totalSizeTextBlock.Text = string.Format("{0} of {1}",
                 new ByteConverter().Convert(TotalUploaded, this.GetType(), null, null).ToString(),
                new ByteConverter().Convert(TotalUploadSize, this.GetType(), null, null).ToString());

            double ByteProcessTime = 0;
            double EstimatedTime = 0;

            try
            {
                TimeSpan timeSpan = DateTime.Now - start;
                ByteProcessTime = TotalUploaded / timeSpan.TotalSeconds;
                EstimatedTime = (TotalUploadSize - TotalUploaded) / ByteProcessTime;
                timeSpan = TimeSpan.FromSeconds(EstimatedTime);
                timeLeftTextBlock.Text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            catch { }
        }

        void upload_StatusChanged(object sender, EventArgs e)
        {
            FileUpload fu = sender as FileUpload;
            if (fu.Status == FileUploadStatus.Complete)
            {
                if (uploading)
                    UploadFiles();
            }
            else if (fu.Status == FileUploadStatus.Removed)
            {
                files.Remove(fu);
                if (uploading)
                    UploadFiles();
            }
            else if (fu.Status == FileUploadStatus.Uploading && files.Count(f => f.Status == FileUploadStatus.Uploading) < MaxConcurrentUploads)
                UploadFiles();
        }

        void FileUploadControl_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.ItemsSource = files;
        }

        private void UploadFiles()
        {

            uploading = true;
            while (files.Count(f => f.Status == FileUploadStatus.Uploading || f.Status == FileUploadStatus.Resizing) < MaxConcurrentUploads && uploading)
            {
                if (files.Count(f => f.Status != FileUploadStatus.Complete && f.Status != FileUploadStatus.Uploading && f.Status != FileUploadStatus.Resizing) > 0)
                {
                    FileUpload fu = files.First(f => f.Status != FileUploadStatus.Complete && f.Status != FileUploadStatus.Uploading && f.Status != FileUploadStatus.Resizing);
                    fu.Upload();
                }
                else if (files.Count(f => f.Status == FileUploadStatus.Uploading || f.Status == FileUploadStatus.Resizing) == 0)
                {
                    uploading = false;
                    uploadButton.Content = " 上 传 ";
                    if (!string.IsNullOrEmpty(JavascriptCompleteFunction))
                    {
                        try
                        {
                            HtmlPage.Window.CreateInstance(JavascriptCompleteFunction);
                        }
                        catch { }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
