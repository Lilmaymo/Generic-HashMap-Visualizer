using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MyHashMape
{

    public enum HashStepType
    {
        ShowBucket,      // Opening the middle detail view
        HighlightIndex,  // Turning the array index Red
        CompareEntry,    // Turning a specific box Orange while searching
        Insert           // Turning the final result Green
    }

    // This holds the DATA for each step
    public class HashStep
    {
        public HashStepType StepType { get; set; }
        public int BucketIndex { get; set; }
        public ChainEntry ComparedEntry { get; set; } // The item currently being looked at
        public string Key { get; set; }              
        public string Value { get; set; }            
    }

    public class HashMapViewModel : INotifyPropertyChanged
    {
        private static readonly Random _rand = new Random();
        public MyHashMap<string, string> Map { get; }
        public ObservableCollection<HashBucketView> Buckets { get; } = new ObservableCollection<HashBucketView>();
        public ObservableCollection<HashStep> Steps { get; set; } = new ObservableCollection<HashStep>();

        private int _currentStepIndex = -1;
        private HashBucketView _selectedBucket;
        private string _stepDescription;
        private bool _isBucketVisible;
        private string _currentSearchValue;
        private string _currentSearchKey;

        public HashBucketView SelectedBucket
        {
            get => _selectedBucket;
            set { _selectedBucket = value; OnPropertyChanged(nameof(SelectedBucket)); }
        }

        public string StepDescription
        {
            get => _stepDescription;
            set { _stepDescription = value; OnPropertyChanged(nameof(StepDescription)); }
        }

        public bool IsBucketVisible
        {
            get => _isBucketVisible;
            set { _isBucketVisible = value; OnPropertyChanged(nameof(IsBucketVisible)); }
        }
        public string CurrentSearchKey
        {
            get => _currentSearchKey;
            set { _currentSearchKey = value; OnPropertyChanged(nameof(CurrentSearchKey)); }
        }

        public string CurrentSearchValue
        {
            get => _currentSearchValue;
            set { _currentSearchValue = value; OnPropertyChanged(nameof(CurrentSearchValue)); }
        }
        public HashMapViewModel(int size)
        {
            Map = new MyHashMap<string, string>(size);
            for (int i = 0; i < size; i++)
                Buckets.Add(new HashBucketView { Index = i });
        }

        public void Add(string key, string value, bool? showSteps = false)
        {
            Steps.Clear();
            int index = Map.GetIndex(key);

            if (showSteps == true)
            {
                Steps.Add(new HashStep { StepType = HashStepType.ShowBucket, BucketIndex = index, Key = key, Value = value });
                Steps.Add(new HashStep { StepType = HashStepType.HighlightIndex, BucketIndex = index, Key = key ,Value = value});

                var entry = Map.GetEntry(index);
                while (entry != null)
                {
                    Steps.Add(new HashStep
                    {
                        StepType = HashStepType.CompareEntry,
                        BucketIndex = index,
                        Key = key,
                        Value= value,
                        ComparedEntry = new ChainEntry { Key = entry.Key, Value = entry.Value }
                    });

                    if (entry.Key == key) break; // Found match: stop searching
                    entry = entry.Next;
                }

                Steps.Add(new HashStep { StepType = HashStepType.Insert, BucketIndex = index, Key = key, Value = value });
                _currentStepIndex = -1;
                NextStep();
            }
            else
            {
                Map.Put(key, value);
                RefreshBucket(index);
            }
        }

        public void NextStep()
        {
            if (_currentStepIndex + 1 < Steps.Count)
            {
                _currentStepIndex++;
                ApplyStep(Steps[_currentStepIndex]);
            }
        }

        public void PreviousStep()
        {
            if (_currentStepIndex > 0)
            {
                _currentStepIndex--;
                ApplyStep(Steps[_currentStepIndex]);
            }
        }

        private void ApplyStep(HashStep step)
        {
            ResetVisuals();
            var bucket = Buckets[step.BucketIndex];

            // Maintain the search context labels
            this.CurrentSearchKey = step.Key;
            this.CurrentSearchValue = step.Value ?? "[DELETE]";

            switch (step.StepType)
            {
                case HashStepType.ShowBucket:
                    SelectedBucket = bucket;
                    IsBucketVisible = true;
                    StepDescription = $"1) Calculated Index {step.BucketIndex} for Key '{step.Key}'. Opening bucket view...";
                    break;

                case HashStepType.HighlightIndex:
                    bucket.IndexColor = "Red";
                    bool isEmpty = string.IsNullOrEmpty(bucket.Key);
                    StepDescription = isEmpty
                        ? $"2) Checking Index {step.BucketIndex}: The bucket is currently EMPTY."
                        : $"2) Checking Index {step.BucketIndex}: There are existing values here. Searching for match...";
                    break;

                case HashStepType.CompareEntry:
                    StepDescription = $"3) Comparing: Checking if '{step.ComparedEntry.Key}' matches target '{step.Key}'...";
                    // Highlight the entry being scanned in Orange
                    if (bucket.Key == step.ComparedEntry.Key) bucket.MainBorderColor = "Orange";
                    else
                    {
                        var target = bucket.Chain.FirstOrDefault(c => c.Key == step.ComparedEntry.Key);
                        if (target != null) target.BorderColor = "Orange";
                    }
                    break;

                case HashStepType.Insert:
                    if (step.Value == null) // This is a Remove operation
                    {
                        // Check if the key exists in the map before removal
                        var entry = Map.GetEntry(step.BucketIndex);
                        bool exists = false;

                        // Traverse to verify existence
                        var current = entry;
                        while (current != null)
                        {
                            if (current.Key == step.Key) { exists = true; break; }
                            current = current.Next;
                        }

                        if (exists)
                        {
                            Map.RemoveEntry(step.Key);
                            RefreshBucket(step.BucketIndex);
                            StepDescription = $"Done! Successfully REMOVED Key '{step.Key}' from the map.";
                        }
                        else
                        {
                            StepDescription = $"Done! Key '{step.Key}' was not found. Nothing to remove.";
                        }
                    }
                    else // This is an Add/Update operation
                    {
                        Map.Put(step.Key, step.Value);
                        RefreshBucket(step.BucketIndex);
                        StepDescription = $"Done! Successfully INSERTED/UPDATED Key '{step.Key}'.";
                        HighlightResult(step.BucketIndex, step.Key, "Green");
                    }
                    break;
            }
        }

        private void HighlightResult(int index, string key, string color)
        {
            var b = Buckets[index];
            if (b.Key == key) b.MainBorderColor = color;
            else
            {
                var target = b.Chain.FirstOrDefault(c => c.Key == key);
                if (target != null) target.BorderColor = color;
            }
        }

        private void ResetVisuals()
        {
            foreach (var b in Buckets)
            {
                b.IndexColor = "Black";
                b.MainBorderColor = "Black";
                foreach (var e in b.Chain) e.BorderColor = "Gray";
            }
        }

        private void RefreshBucket(int index)
        {
            var view = Buckets[index];
            view.Chain.Clear();
            var entry = Map.GetEntry(index);
            if (entry == null) { view.Key = ""; view.Value = ""; return; }

            view.Key = entry.Key;
            view.Value = entry.Value;
            var next = entry.Next;
            while (next != null)
            {
                view.Chain.Add(new ChainEntry { Key = next.Key, Value = next.Value });
                next = next.Next;
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            Steps.Clear();
            int index = Map.GetIndex(key);

            // 1. Always show the bucket and highlight index first
            Steps.Add(new HashStep { StepType = HashStepType.ShowBucket, BucketIndex = index, Key = key, Value = null });
            Steps.Add(new HashStep { StepType = HashStepType.HighlightIndex, BucketIndex = index, Key = key, Value = null });

            var entry = Map.GetEntry(index);
            bool found = false;

            // 2. Generate comparison steps if there are items
            while (entry != null)
            {
                Steps.Add(new HashStep
                {
                    StepType = HashStepType.CompareEntry,
                    BucketIndex = index,
                    Key = key,
                    Value = null,
                    ComparedEntry = new ChainEntry { Key = entry.Key, Value = entry.Value }
                });

                if (entry.Key == key) { found = true; break; }
                entry = entry.Next;
            }

            // 3. FIX: Always add the final "Result" step so the UI doesn't hang at Step 2
            Steps.Add(new HashStep { StepType = HashStepType.Insert, BucketIndex = index, Key = key, Value = null });

            _currentStepIndex = -1;
            NextStep();
        }
        public void AutoFill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Generate a random 4-letter key and 4-letter value
                string key = GenerateRandomKey(4);
                string value = GenerateRandomString(4);

                // Put into map
                Map.Put(key, value);

                // Update the specific bucket view
                RefreshBucket(Map.GetIndex(key));
            }
            StepDescription = $"Auto-filled {count} random elements.";
        }
        private string GenerateRandomKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable
                .Repeat(chars, length)
                .Select(s => s[_rand.Next(s.Length)])
                .ToArray());
        }

        private string GenerateRandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_rand.Next(chars.Length)];
            }

            return new string(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}