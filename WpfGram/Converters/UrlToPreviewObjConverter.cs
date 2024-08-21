using System;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

using WpfGram.Models;

namespace WpfGram.Converters
{
    public class UrlToPreviewObjConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (value == null)
                return null;
            var task = Task.Run(() => (GetPreviewObject(value.ToString())));
            return new TaskCompletionNotifier<UrlPreviewModel>(task);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private async Task<UrlPreviewModel> GetPreviewObject(string url)
        {

            string data = "";
            UrlPreviewModel previewObj = new();
            using (HttpClient web1 = new HttpClient())
                data = await web1.GetStringAsync(new Uri(url));

            // first define what you will look for using regex pattern syntax
            var p = @"<meta\s{1,}property=""og:image""\s{1,}content=""(.+)""(.*)>";
            var t = @"<meta\s{1,}property=""og:title""\s{1,}content=""(.+)""(.*)>";
            var d = @"<meta\s{1,}property=""og:description""\s{1,}content=""(.+)""(.*)>";

            // second let the regex engine use your pattern against your html string
            var m = System.Text.RegularExpressions.Regex.Match(data, p);
            var desc = System.Text.RegularExpressions.Regex.Match(data, d);
            var title = System.Text.RegularExpressions.Regex.Match(data, t);

            // third pull out just the part you want from the resulting match
            var g = m.Groups[1];
            previewObj.ImageUrl = g.Value.Split('"')[0];
            previewObj.Description = desc.Groups[1].Value.Split('>')[0];
            previewObj.Title = title.Groups[1].Value.Split('"')[0];
            return previewObj;
        }
    }
    public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        public TaskCompletionNotifier(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                        if (t.IsCanceled)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                        }
                        else if (t.IsFaulted)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        }
                        else
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                            propertyChanged(this, new PropertyChangedEventArgs("Result"));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler);
            }
        }

        // Gets the task being watched. This property never changes and is never <c>null</c>.
        public Task<TResult> Task { get; private set; }



        // Gets the result of the task. Returns the default value of TResult if the task has not completed successfully.
        public TResult Result { get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult); } }

        // Gets whether the task has completed.
        public bool IsCompleted { get { return Task.IsCompleted; } }

        // Gets whether the task has completed successfully.
        public bool IsSuccessfullyCompleted { get { return Task.Status == TaskStatus.RanToCompletion; } }

        // Gets whether the task has been canceled.
        public bool IsCanceled { get { return Task.IsCanceled; } }

        // Gets whether the task has faulted.
        public bool IsFaulted { get { return Task.IsFaulted; } }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
