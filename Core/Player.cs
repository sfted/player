using Player.Core.Entities;
using Player.Core.Entities.Interfaces;
using Player.Core.Utils.Extensions;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Media;

namespace Player.Core
{
    public class Player : Notifier
    {
        private readonly MediaPlayer player = new MediaPlayer();
        private readonly Timer timer = new Timer();

        public PlaybackQueue Queue { get; private set; }

        private Track currentTrack;
        public Track CurrentTrack
        {
            get => currentTrack;
            set
            {
                currentTrack = value;
                IsTrackSet = true;
                NotifyPropertyChanged(nameof(CurrentTrack));
            }
        }

        private bool isTrackSet;
        public bool IsTrackSet
        {
            get => isTrackSet;
            set
            {
                isTrackSet = value;
                NotifyPropertyChanged(nameof(IsTrackSet));
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                NotifyPropertyChanged(nameof(IsPlaying));
            }
        }

        public double PositionInSeconds
        {
            get => player.Position.TotalSeconds;
            set => player.Position = TimeSpan.FromSeconds(value);
        }

        public double Volume
        {
            get => player.Volume;
            set => player.Volume = value;
        }

        public TimeSpan Position
        {
            get => player.Position;
        }

        private RelayCommand openAndPlayCommand;
        public RelayCommand OpenAndPlayCommand
        {
            get => openAndPlayCommand ??= new RelayCommand
            (
                obj =>
                {
                    if (obj is Track track)
                    {
                        if (CurrentTrack != null && track.Id == CurrentTrack.Id)
                            PlayPause();
                        else
                            PlayNewTrack(track);
                    }
                    else if (obj is object[] parameters)
                    {
                        var track1 = (Track)parameters[0];

                        if (CurrentTrack != null && track1.Id == CurrentTrack.Id)
                        {
                            PlayPause();
                        }
                        else
                        {
                            if (parameters[1] is IPlayable iPlayable)
                                PlayNewTrack(track1, iPlayable);
                            else if (parameters[1] is List<Track> tracks)
                                PlayNewTrack(track1, tracks);
                            //else if (parameters[1] is ObservableCollection<Track> queue)
                            //{
                            //    CurrentTrackIndexInQueue = PlaybackQueue.IndexOf(track1);
                            //    OpenTrack(track1);
                            //    PlayPause(true);
                            //}
                        }
                    }
                    else if (obj is IPlayable collection)
                    {
                        PlayNewTrack(collection);
                    }
                }
            );
        }

        //private RelayCommand cyclePlaybackModeCommand;
        //public RelayCommand CyclePlaybackModeCommand
        //{
        //    get => cyclePlaybackModeCommand ??= new RelayCommand
        //    (
        //        obj => CyclePlaybackMode(),
        //        obj => CanManagePlayback()
        //    );
        //}

        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get => playPauseCommand ??= new RelayCommand
            (
                obj => PlayPause(),
                obj => CanManagePlayback()
            );
        }

        private RelayCommand skipToNextCommand;
        public RelayCommand SkipToNextCommand
        {
            get => skipToNextCommand ??= new RelayCommand
            (
                obj => SkipToNext(),
                obj => CanManagePlayback() && Queue.CanSkipToNextTrack()
            );
        }

        private RelayCommand skipToPreviousCommand;
        public RelayCommand SkipToPreviousCommand
        {
            get => skipToPreviousCommand ??= new RelayCommand
            (
                obj => SkipToPrevious(),
                obj => CanManagePlayback()
            );
        }

        public Player()
        {
            IsPlaying = false;
            timer.Interval = 100;
            timer.Elapsed += (s, e) =>
            {
                NotifyPropertyChanged(nameof(PositionInSeconds));
                NotifyPropertyChanged(nameof(Position));
            };

            player.MediaEnded += (s, e) =>
            {
                if (Queue.CanSkipToNextTrack())
                    SkipToNext();
            };

            player.MediaFailed += (s, e) =>
            {
                if (Queue.CanSkipToNextTrack())
                    SkipToNext();
            };
        }

        private void PlayNewTrack(Track track)
        {
            Queue = new PlaybackQueue(track);
            OpenTrack(track);
            PlayPause(true);
        }

        private void PlayNewTrack(IPlayable trackCollection)
        {
            var collection = new ObservableCollection<Track>();
            foreach (var t in trackCollection.Tracks)
                collection.Add(t);

            Queue = new PlaybackQueue(collection);

            OpenTrack(Queue[0]);
            PlayPause(true);
        }

        private void PlayNewTrack(Track track, IPlayable trackCollection)
        {
            var collection = new ObservableCollection<Track>();
            foreach (var t in trackCollection.Tracks)
                collection.Add(t);

            Queue = new PlaybackQueue(track, collection);

            OpenTrack(track);
            PlayPause(true);
        }

        private void PlayNewTrack(Track track, List<Track> tracks)
        {
            var collection = new ObservableCollection<Track>();
            foreach (var t in tracks)
                collection.Add(t);

            Queue = new PlaybackQueue(track, collection);

            OpenTrack(track);
            PlayPause(true);
        }

        //private void CyclePlaybackMode()
        //{
        //    if (PlaybackMode == PlaybackModes.Shuffle)
        //        PlaybackMode = PlaybackModes.Default;
        //    else
        //        PlaybackMode++;
        //}

        private void OpenTrack(Track track)
        {
            CurrentTrack = track;
            player.Open(new Uri(track.FileName, UriKind.Absolute));
        }

        private void PlayPause(bool playForcibly = false)
        {
            if (playForcibly)
            {
                player.Play();
                IsPlaying = true;
                timer.Start();
            }
            else
            {
                if (IsPlaying)
                {
                    player.Pause();
                    IsPlaying = false;
                    timer.Stop();
                }
                else
                {
                    player.Play();
                    IsPlaying = true;
                    timer.Start();
                }
            }
        }

        private void SkipToNext()
        {
            OpenTrack(Queue.GetNextTrack());
            PlayPause(true);
        }

        private void SkipToPrevious()
        {
            OpenTrack(Queue.GetPreviousTrack());
            PlayPause(true);
        }

        private bool CanManagePlayback()
        {
            return CurrentTrack != null;
        }

        public enum PlaybackModes
        {
            Default,
            RepeatAll,
            RepeatOne,
            Shuffle
        }
    }
}
