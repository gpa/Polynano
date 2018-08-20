/*
MIT License

Copyright(c) 2018 Gratian Pawliszyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Polynano.Rendering;
using Polynano.Startup.Utils;
using Polynano.Startup.ViewModels;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polynano.Startup.Views
{
    public partial class MainView : Form
    {
        public ApplicationViewModel ApplicationViewModel { get; private set; }

        private MeshViewControl _meshViewControl;

        private MeshViewControlController _meshViewControlController;

        public MainView(ApplicationViewModel applicationViewModel)
        {
            ApplicationViewModel = applicationViewModel;
            InitializeComponent();
            _meshViewControl = new MeshViewControl(this);
            _meshViewControl.OnReady += OnMeshViewControlReady;
            _meshViewControlController = new MeshViewControlController(_meshViewControl);
            OnResize(EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            if (_meshViewControlController != null)
                _meshViewControlController.Resize(leftPanel.Width, 0, ClientSize.Width - leftPanel.Width, ClientSize.Height);
        }

        private async void LoadButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            if (string.IsNullOrEmpty(openFileDialog.FileName))
                return;

            if (new FileInfo(openFileDialog.FileName).Length < Constants.MinFileSizeForLoadingDialog)
            {
                ApplicationViewModel.Load(openFileDialog.FileName);
                ApplicationViewModel.InitializeSimplifier();
                _meshViewControlController.SetMesh(ApplicationViewModel.ProcessingMesh);
            }
            else
            {
                await ProvideLoadingFormForAction(p =>
                {
                    ApplicationViewModel.Load(openFileDialog.FileName, p);
                    Thread.Sleep(700);
                }, $"{openFileDialog.SafeFileName} {Constants.IsBeingLoadedMessage}");

                await ProvideLoadingFormForAction(p =>
                {
                    ApplicationViewModel.InitializeSimplifier();
                    Thread.Sleep(100);
                }, Constants.ProcessingGeometryMessage, ProgressBarStyle.Marquee);
            }

            _meshViewControlController.SetMesh(ApplicationViewModel.ProcessingMesh);

            complexityTrackbar.Value = complexityTrackbar.Maximum;
            simplifyButton.Enabled = true;
            complexityTrackbar.Enabled = true;
            ShowVerticesCheckbox.Enabled = true;
            ShowFacesCheckbox.Enabled = true;
            ShowEdgesCheckbox.Enabled = true;
            modelStatsTable.Visible = true;
            UpdateStatistics();
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = $"Simplified_{Path.GetFileName(ApplicationViewModel.OriginalFileName)}.ply",
                AddExtension = true,
                DefaultExt = "ply"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // If the model is relativly small, do not show the progress window
            if (ApplicationViewModel.ProcessingMesh.Faces.Count < Constants.MinModelFaceCountForSavingDialog)
            {
                ApplicationViewModel.Save(dialog.FileName);
            }
            else
            {
                // Show the progress window
                await ProvideLoadingFormForAction(p =>
                {
                    ApplicationViewModel.Save(dialog.FileName, p);
                    // Wait 1/10 of a second for the progress bar animation to go to 100 %
                    Thread.Sleep(100);
                }, $"{ApplicationViewModel.OriginalFileName} {Constants.SavingMessage} ...");
            }

            MessageBox.Show($"{Path.GetFileName(dialog.FileName)} {Constants.WasSuccessfullySavedMessage}", Constants.SuccessMessage);
        }

        private void OnMeshViewControlReady(object sender, EventArgs e)
        {
            _meshViewControlController.Initialize();
        }

        public new void Dispose()
        {
            base.Dispose();
            _meshViewControl.Dispose();
            _meshViewControlController.Dispose();
        }

        private void ShowFacesCheckbox_CheckedChanged(object sender, EventArgs e)
            => _meshViewControlController.DisplayFaces = ShowFacesCheckbox.Checked;

        private void ShowEdgesCheckbox_CheckedChanged(object sender, EventArgs e)
            => _meshViewControlController.DisplayEdges = ShowEdgesCheckbox.Checked;

        private void ShowVerticesCheckbox_CheckedChanged(object sender, EventArgs e)
            => _meshViewControlController.DisplayVertices = ShowVerticesCheckbox.Checked;

        private async void simplifyButton_Click(object sender, EventArgs e)
        {
            if (ApplicationViewModel.ProcessingMesh == null)
                return;

            saveButton.Enabled = true;
            simplifyButton.Enabled = false;
            complexityTrackbar.Enabled = false;


            if (ApplicationViewModel.OriginalVertexCount > 70000)
            {
                await SimplifyInstantly(complexityTrackbar.Value);
                _meshViewControlController.SetMesh(ApplicationViewModel.ProcessingMesh);
                simplifyButton.Enabled = true;
                complexityTrackbar.Enabled = true;
            }
            else
            {
                simplificationTimer.Interval = 80;
                simplificationTimer.Enabled = true;
            }
        }

        private static async Task ProvideLoadingFormForAction(Action<IProgress<int>> action, string title, ProgressBarStyle style = ProgressBarStyle.Continuous)
        {
            var loadingForm = new LoadingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                StatusText = title,
                Style = style
            };
            loadingForm.Show();
            var progressHandler = new Progress<int>(value => { loadingForm.StatusBar = value; });
            var progress = (IProgress<int>)progressHandler;
            await Task.Run(() => action(progress));
            loadingForm.Dispose();
        }

        private async Task SimplifyInstantly(int targetComplexity)
        {
            await ProvideLoadingFormForAction(p =>
            {
                ApplicationViewModel.Simplify((float)targetComplexity);
            }, Constants.PleaseWaitMessage, ProgressBarStyle.Marquee);
            UpdateStatistics();
        }

        private void OnSimplificationTimerTick(object sender, EventArgs e)
        {
            var verticesToRemoveProTick = Math.Max((int)(ApplicationViewModel.ProcessingMesh.Vertices.Count * 0.09f), 1);

            if (ApplicationViewModel.Simplify(complexityTrackbar.Value, verticesToRemoveProTick))
            {
                simplificationTimer.Enabled = false;
                simplifyButton.Enabled = true;
                complexityTrackbar.Enabled = true;
                UpdateStatistics();
                return;
            }

            _meshViewControlController.SetMesh(ApplicationViewModel.ProcessingMesh);
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            statsSizeLabel.Text = GetSizeForModel(ApplicationViewModel.ProcessingMesh.Faces.Count, ApplicationViewModel.ProcessingMesh.Vertices.Count);
            StatsFaceCountLabel.Text = ApplicationViewModel.ProcessingMesh.Faces.Count.ToString();
            StatsVertexCountLabel.Text = ApplicationViewModel.ProcessingMesh.Vertices.Count.ToString();

            float percent = ApplicationViewModel.ProcessingMesh.Vertices.Count * 100f / ApplicationViewModel.OriginalVertexCount;
            string p = percent.ToString();

            if (p.EndsWith(",00"))
            {
                p = p.Substring(p.Length - 3, 3);
            }

            statsComplexityLabel.Text = percent.ToString("n2") + "%";
        }

        private string GetSizeForModel(int faceCount, int vertexCount)
        {
            float estimatedSize = (faceCount * 4 * sizeof(int))
                                  + (vertexCount * 3 * sizeof(float)) + 203; // 203 is the header size. Note that 203 is an estimate. 
            // in the header the number of faces and vertices is stored and depending on how big the number is, more characters will 
            // need to be stored therefore increasing the header size. It is possible to compute the exact number of bytes needed, however I don't care.

            estimatedSize /= 1048576; // convert bytes to mb
            return estimatedSize.ToString("0.##") + @" MB";
        }
    }
}
