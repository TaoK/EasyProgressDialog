/*
 * This file is part of EasyProgressDialog. EasyProgressDialog is free software: you can redistribute 
 * it and/or modify it under the terms of the GNU Lesser Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) any later version. 
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even 
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser 
 * Public License for more details. You should have received a copy of the GNU Lesser Public License
 * along with EasyProgressDialog.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace KlerksSoft.EasyProgressDialog
{
    /* ProgressDialog - simple generic progress dialog
     * 
     * Generic progress dialog, displaying items as they occur and receiving only 
     * simple messages for updates.
     * 
     * Advantage: easy to add incremental cancellable work handling without mixing 
     * specific business logic and UI code. The business logic only needs to be aware 
     * of this single generic UI class, with a single line of code at every progress 
     * checkpoint.
     * 
     * There are two methods that can be called within the "work" code (when there is 
     * a progress dialog object present):
     * 
     * Worker_IncrementProgress(string currentAction)
     *   - sets the "current action" area of the dialog with the provided text (if 
     *       set), and increments the progress by 1. If this looks like it will reach
     *       the maximum, then it increments the maximum (so you never reach the end
     *       of the progress bar by calling this).
     * 
     * Worker_SetSpecificProgress(string currentAction, int currentCount, int totalEstimatedCount)
     *   - allows you to adjust the overall estimate. This is useful if you learn 
     *       more about the work to be done as you do it, or if you have no information
     *       on which to base your estimate when you originally bring up the dialog.
     * 
     * You can also change the dialog preferences at any time, they will only 
     * actually "kick in" at the next progress update - in this manner all exposed
     * properties are safe to be called from the Worker Thread (whatever is called 
     * from the "Do Work" method passed in).
     * 
     */

    /// <summary>
    /// Allows for easy display of cancellable progress dialog, with minimal changes to 
    /// "work" code. Uses <see cref="System.ComponentModel.BackgroundWorker"/> to queue
    /// the work and and a simple <see cref="System.Windows.Forms.Form"/> for the dialog.
    /// </summary>
    public class ProgressDialog
    {
        private BackgroundWorker _bgWorker;
        private DoWorkEventArgs _currentBgWorkEventArgs;

        /// <summary>
        /// Defines the type that work queueing delegates will need to implement.
        /// </summary>
        /// <param name="workArgument">A single object that will be passed to the work method/delegate</param>
        public delegate void WorkMethod(object workArgument);

        private WorkMethod _doBgWork;

        private Exception _exceptionWhileWorking = null;

        private ProgressDialogForm _dialogForm;

        /// <summary>
        /// The time that processing started, and that ETA is calculated from.
        /// </summary>
        public DateTime StartDateTime { get; set; }
        private DateTime? _lastDisplayUpdate = null;

        /// <summary>
        /// The title of the dialog.
        /// </summary>
        public string ActionTitle { get; set; }

        /// <summary>
        /// The current action / status, displayed at the top of the form. Used to display detail of specific processing going on at that time.
        /// </summary>
        public string CurrentAction { get; set; }

        /// <summary>
        /// The current count of action items performed. This is compared to the TotalEstimatedCount to determine estimated completion.
        /// </summary>
        public long CurrentCount { get; set; }

        /// <summary>
        /// The estimated number of work items to be performed. This can be adjusted while work is ongoing, and it automatically increases if CurrentCount reaches it - the assumption being that a progress dialog will never actually reach 100% completion (until it is completed and closes).
        /// </summary>
        public long TotalEstimatedCount { get; set; }

        /// <summary>
        /// Determine whether you want to display current count and estimated total - this only makes sense when you think your estimated total is relatively reliable (people hate to see the amount of work go up as part of the work gets done).
        /// </summary>
        public bool DisplayCounts { get; set; }

        /// <summary>
        /// Determine whether you want to display ETA - this only makes sense if the work is relatively regular and the ETAs are therefore meaningful. 
        /// </summary>
        public bool DisplayTimeEstimates { get; set; }

        /// <summary>
        /// The delay from the start of processing before ETAs are first displayed (if enabled - see DisplayTimeEstimates). 
        /// </summary>
        public double TimeEstimateInitialDelaySeconds { get; set; }

        /// <summary>
        /// The minimum delay between UI refreshes. If progress updates are more frequent they simply won't trigger a UI update. This is important to avoid UI refreshes taking up too much CPU time and actually slowing down the work to be done.
        /// </summary>
        public double DisplayIntervalSeconds { get; set; }

        /// <summary>
        /// Standard constructor, preparing the dialog object for use.
        /// </summary>
        public ProgressDialog()
        {
            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = true;
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            _bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);

            DisplayCounts = true;
            DisplayTimeEstimates = true;
            TimeEstimateInitialDelaySeconds = 2;
            DisplayIntervalSeconds = 0.5;
        }

        private void UpdateDisplay()
        {
            if (_lastDisplayUpdate == null || (DateTime.Now - _lastDisplayUpdate.Value).TotalSeconds > DisplayIntervalSeconds)
            {
                _dialogForm.Text = ActionTitle;

                _dialogForm.progressBar1.Value = (int)((CurrentCount * 1.0 / TotalEstimatedCount) * 10000);

                if (!string.IsNullOrEmpty(CurrentAction))
                    _dialogForm.txt_CurrentAction.Text = CurrentAction;

                if (DisplayCounts && TotalEstimatedCount > 1)
                {
                    _dialogForm.lbl_Progress.Text = CurrentCount.ToString() + " / " + TotalEstimatedCount.ToString();
                }
                else
                {
                    _dialogForm.lbl_Progress.Text = "";
                }

                TimeSpan timeElapsed = (DateTime.Now - StartDateTime);
                if (DisplayTimeEstimates && TotalEstimatedCount > 1
                    && timeElapsed.TotalSeconds > TimeEstimateInitialDelaySeconds
                    && CurrentCount > 0)
                {
                    double rateSoFar = CurrentCount / timeElapsed.TotalSeconds;
                    TimeSpan expectedTime = TimeSpan.FromSeconds((TotalEstimatedCount - CurrentCount) / rateSoFar);
                    _dialogForm.lbl_TimeEstimate.Text = Utils.GetApproxTimeSpanDescription(expectedTime) + " remaining.";
                }
                else
                {
                    _dialogForm.lbl_TimeEstimate.Text = "";
                }

                _lastDisplayUpdate = DateTime.Now;
            }
        }

        /// <summary>
        /// Displays a modal progress dialog form and kicks off a work method using a BackgroundWorker; the modal progress dialog form will remain in the foreground until the user presses cancel AND a checkpoint in the work code is reached, or until the work completes.
        /// </summary>
        /// <param name="actionTitle">The title of the dialog form/window</param>
        /// <param name="initialCurrentAction">The initial status text in the window</param>
        /// <param name="estimatedCount">The estimated total number of work items to be performed</param>
        /// <param name="workMethod">The delegate method that will actually do the work</param>
        /// <param name="bgWorkerArgument">The object that will get passed to the delegate</param>
        /// <returns></returns>
        public DialogResult StartProgressDialog(string actionTitle, string initialCurrentAction, long estimatedCount, WorkMethod workMethod, object bgWorkerArgument)
        {
            if (_dialogForm == null)
            {
                _dialogForm = new ProgressDialogForm();
                _dialogForm.btn_Cancel.Click += new EventHandler(btn_Cancel_Click);
            }
            else
                throw new Exception("This class can only display one dialog at a time, per instance of the class.");

            ActionTitle = actionTitle;
            TotalEstimatedCount = estimatedCount;
            CurrentCount = 0;
            if (!string.IsNullOrEmpty(initialCurrentAction))
                CurrentAction = initialCurrentAction;
            else
                CurrentAction = "";
            StartDateTime = DateTime.Now;

            UpdateDisplay();

            //record the work method
            _doBgWork += workMethod;

            //start the worker
            _bgWorker.RunWorkerAsync(bgWorkerArgument);

            //block the underlying UI (and code flow) by showing this modal form.
            return _dialogForm.ShowDialog();
        }

        /// <summary>
        /// Increment the current progress count, and detect whether a cancellation has been queued.
        /// </summary>
        /// <returns>true if progress is successfully incremented, false if a cancellation is detected</returns>
        public bool Worker_IncrementProgress()
        {
            return Worker_SetSpecificProgress(null, null, null);
        }

        /// <summary>
        /// Increment the current progress count, detect whether a cancellation has been queued, and set the status text (eg to a filename or object name being worked on)
        /// </summary>
        /// <param name="currentAction">Status text to display on the progress dialog</param>
        /// <returns>true if progress is successfully incremented, false if a cancellation is detected</returns>
        public bool Worker_IncrementProgress(string currentAction)
        {
            return Worker_SetSpecificProgress(currentAction, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentAction">Status text to display on the progress dialog</param>
        /// <param name="currentCount">Specific progress count to set</param>
        /// <param name="totalEstimatedCount">Specific total work estimate to set</param>
        /// <returns>true if progress is successfully set, false if a cancellation is detected</returns>
        public bool Worker_SetSpecificProgress(string currentAction, long? currentCount, long? totalEstimatedCount)
        {
            if (_bgWorker.CancellationPending)
            {
                _currentBgWorkEventArgs.Cancel = true;
                return false; //did not find reason to continue and record progress
            }

            //update the action to display
            CurrentAction = currentAction;

            //update the max if provided.
            if (totalEstimatedCount != null && totalEstimatedCount.Value > CurrentCount)
                TotalEstimatedCount = totalEstimatedCount.Value;

            //if current count is not set, increment
            if (currentCount == null)
            {
                //if we're reaching the max, keep away from it.
                if (CurrentCount >= TotalEstimatedCount - 1)
                    TotalEstimatedCount++;

                //actually increment the progress
                CurrentCount++;
            }
            else
            {
                CurrentCount = currentCount.Value;
            }

            _bgWorker.ReportProgress(0, null);
            return true; //did find reason to continue and record progress.
        }

        #region BG Worker Events
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _currentBgWorkEventArgs = e;
            try
            {
                _doBgWork(e.Argument);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                _exceptionWhileWorking = ex;
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateDisplay();
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                _dialogForm.DialogResult = DialogResult.Cancel;
            else
                _dialogForm.DialogResult = DialogResult.OK;

            _doBgWork = null;
            _lastDisplayUpdate = null;

            _dialogForm.Close();
            _dialogForm.Dispose();
            _dialogForm = null;

            if (_exceptionWhileWorking != null)
            {
                Exception localReferenceToException = _exceptionWhileWorking;
                _exceptionWhileWorking = null;
                throw new Exception("Exception encountered while executing provided work method in ProgressDialog class.", localReferenceToException);
            }
        }
        #endregion

        #region ProgressDialogForm Events
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (_bgWorker != null)
                _bgWorker.CancelAsync();
        }
        #endregion
    }
}
