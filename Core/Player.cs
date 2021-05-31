using Player.Core.Entities;
using Player.Core.Entities.Interfaces;
using Player.Core.Utils;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Media;

namespace Player.Core
{
    public class Player : Notifier
    {
        private readonly MediaPlayer player = new MediaPlayer();
        private readonly Timer timer = new Timer();

        private PlaybackQueue queue;
        public PlaybackQueue Queue
        {
            get => queue;
            set
            {
                queue = value;
                queue.RepeatMode = Settings.RepeatMode;
                queue.NowPlayingTrackChanged += (t) => OpenTrack(Queue.NowPlayingTrack);
                NotifyPropertyChanged(nameof(Queue));
                OpenTrack(Queue.NowPlayingTrack);
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
            set
            {
                player.Volume = value;
                Settings.Volume = value;
            }
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
                    if (obj is object[] parameters)
                    {
                        var track1 = (Track)parameters[0];
                        if (Queue != null && track1.Id == Queue.NowPlayingTrack.Id) PlayPause();
                        else
                        {
                            if (parameters[1] is IPlayable collection)
                            {
                                if (Queue != null && collection.GetHashCode() == queue.OpenedListHashCode)
                                    queue.SkipToTrack(track1);
                                else
                                    PlayNewQueue(track1, collection);
                            }
                            else if (parameters[1] is List<Track> tracks)
                            {
                                if (Queue != null && tracks.GetHashCode() == queue.OpenedListHashCode)
                                    queue.SkipToTrack(track1);
                                else
                                    PlayNewQueue(track1, tracks);
                            }
                            else if (parameters[1] is PlaybackQueue queue) queue.SkipToTrack(track1);
                        }
                    }
                    else if (obj is IPlayable collection) PlayNewQueue(collection);
                }
            );
        }

        private RelayCommand switchRepeatModeCommand;
        public RelayCommand SwitchRepeatModeCommand
        {
            get => switchRepeatModeCommand ??= new RelayCommand
            (
                obj =>
                {
                    Queue.SwitchRepeatMode();
                    Settings.RepeatMode = Queue.RepeatMode;
                },
                obj => CanManagePlayback()
            );
        }

        private RelayCommand toggleShuffleCommand;
        public RelayCommand ToggleShuffleCommand
        {
            get => toggleShuffleCommand ??= new RelayCommand
            (
                obj =>
                {
                    Queue.ToggleShuffle();
                    Settings.ShuffleIsEnabled = Queue.ShuffleIsEnabled;
                },
                obj => CanManagePlayback()
            );
        }

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
                obj => Queue.SkipToNextTrack(forced: true),
                obj => CanManagePlayback() && Queue.CanSkipToNextTrack()
            );
        }

        private RelayCommand skipToPreviousCommand;
        public RelayCommand SkipToPreviousCommand
        {
            get => skipToPreviousCommand ??= new RelayCommand
            (
                obj => Queue.SkipToPreviousTrack(),
                obj => CanManagePlayback()
            );
        }

        public Player()
        {
            Volume = Settings.Volume;
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
                    Queue.SkipToNextTrack();
            };

            player.MediaFailed += (s, e) =>
            {
                if (Queue.CanSkipToNextTrack())
                    Queue.SkipToNextTrack();
            };
        }

        private void PlayNewQueue(IPlayable collection) => Queue = new PlaybackQueue(collection.Tracks, Settings.ShuffleIsEnabled);
        private void PlayNewQueue(Track track, IPlayable collection) => Queue = new PlaybackQueue(track, collection.Tracks, Settings.ShuffleIsEnabled);
        private void PlayNewQueue(Track track, List<Track> tracks) => Queue = new PlaybackQueue(track, tracks, Settings.ShuffleIsEnabled);

        private void OpenTrack(Track track)
        {
            player.Open(new Uri(track.FileName, UriKind.Absolute));
            PlayPause(true);
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

        private bool CanManagePlayback()
        {
            return Queue != null;
        }
    }
}
