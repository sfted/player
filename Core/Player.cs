using Player.Core.Entities;
using Player.Core.Entities.Interfaces;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Media;

namespace Player.Core
{
    public class Player : Notifier
    {
        private readonly MediaPlayer player = new MediaPlayer();
        private readonly Timer timer = new Timer();

        public ObservableCollection<Track> PlaybackQueue { get; private set; }
        public int CurrentTrackIndexInQueue { get; private set; }

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

        private PlaybackModes playbackMode;
        public PlaybackModes PlaybackMode
        {
            get => playbackMode;
            set
            {
                playbackMode = value;
                NotifyPropertyChanged(nameof(PlaybackMode));
            }
        }

        public double PositionInSeconds
        {
            get => player.Position.TotalSeconds;
            set => player.Position = TimeSpan.FromSeconds(value);
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
                            else if (parameters[1] is ObservableCollection<Track> queue)
                            {
                                CurrentTrackIndexInQueue = PlaybackQueue.IndexOf(track1);
                                OpenTrack(track1);
                                PlayPause(true);
                            }
                        }
                    }
                }
            );
        }

        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get => playPauseCommand ??= new RelayCommand
            (
                obj => PlayPause(),
                obj => CanIManagePlayback()
            );
        }

        private RelayCommand skipToNextCommand;
        public RelayCommand SkipToNextCommand
        {
            get => skipToNextCommand ??= new RelayCommand
            (
                obj => SkipToNext(),
                obj => CanIManagePlayback() && CanIGoToNext()
            );
        }

        private RelayCommand skipToPreviousCommand;
        public RelayCommand SkipToPreviousCommand
        {
            get => skipToPreviousCommand ??= new RelayCommand
            (
                obj => SkipToPrevious(),
                obj => CanIManagePlayback() && CanIGoToPrevious()
            );
        }

        public Player()
        {
            PlaybackMode = PlaybackModes.RepeatAll;
            IsPlaying = false;
            timer.Interval = 100;
            timer.Elapsed += (s, e) =>
            {
                NotifyPropertyChanged(nameof(PositionInSeconds));
                NotifyPropertyChanged(nameof(Position));
            };

            player.MediaEnded += (s, e) =>
            {
                if (PlaybackMode == PlaybackModes.RepeatOne)
                    SkipToNext(stayOnCurrentTrack: true);
                else if (CanIGoToNext())
                    SkipToNext();
            };

            player.MediaFailed += (s, e) =>
            {
                if (CanIGoToNext())
                    SkipToNext();
            };
        }

        private void PlayNewTrack(Track track)
        {
            PlaybackQueue = new ObservableCollection<Track>();
            PlaybackQueue.Add(track);
            CurrentTrackIndexInQueue = 0;
            OpenTrack(track);
            PlayPause(true);
        }

        private void PlayNewTrack(Track track, IPlayable trackCollection)
        {
            PlaybackQueue = new ObservableCollection<Track>();

            foreach (var t in trackCollection.Tracks)
                PlaybackQueue.Add(t);

            CurrentTrackIndexInQueue = PlaybackQueue.IndexOf(track);

            OpenTrack(track);
            PlayPause(true);
        }

        private void PlayNewTrack(Track track, List<Track> tracks)
        {
            PlaybackQueue = new ObservableCollection<Track>();

            foreach (var t in tracks)
                PlaybackQueue.Add(t);

            CurrentTrackIndexInQueue = PlaybackQueue.IndexOf(track);

            OpenTrack(track);
            PlayPause(true);
        }

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

        private void SkipToNext(bool stayOnCurrentTrack = false)
        {
            if (!stayOnCurrentTrack)
            {
                if (CurrentTrackIndexInQueue < PlaybackQueue.Count - 1)
                    CurrentTrackIndexInQueue++;
                else
                    CurrentTrackIndexInQueue = 0;

                OpenTrack(PlaybackQueue[CurrentTrackIndexInQueue]);
            }
            else
                player.Position = TimeSpan.FromSeconds(0);

            PlayPause(true);
        }

        private void SkipToPrevious()
        {
            CurrentTrackIndexInQueue--;
            OpenTrack(PlaybackQueue[CurrentTrackIndexInQueue]);
            PlayPause(true);
        }

        private bool CanIManagePlayback()
        {
            return CurrentTrack != null;
        }

        private bool CanIGoToPrevious()
        {
            return CurrentTrackIndexInQueue > 0;
        }

        private bool CanIGoToNext()
        {
            if (PlaybackMode == PlaybackModes.RepeatAll)
                return true;
            else
                return CurrentTrackIndexInQueue < PlaybackQueue.Count - 1;
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
