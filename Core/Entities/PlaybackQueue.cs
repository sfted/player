using Player.Core.Utils.Extensions;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Player.Core.Entities
{
    public class PlaybackQueue : Notifier
    {
        private List<int> unshuffledQueueTrackIds;
        private int nowPlayingTrackIndex;

        public ObservableCollection<Track> Tracks { get; private set; }

        public Action<Track> NowPlayingTrackChanged;

        public int OpenedListHashCode { get; private set; }

        private bool shuffleIsEnabled = false;
        public bool ShuffleIsEnabled
        {
            get => shuffleIsEnabled;
            set
            {
                shuffleIsEnabled = value;
                NotifyPropertyChanged(nameof(ShuffleIsEnabled));

                if (shuffleIsEnabled)
                    Shuffle();
                else
                    Unshuffle();
            }
        }

        private Track nowPlayingTrack;
        public Track NowPlayingTrack
        {
            get => nowPlayingTrack;
            private set
            {
                nowPlayingTrack = value;
                NotifyPropertyChanged(nameof(NowPlayingTrack));

                if (NowPlayingTrackChanged != null)
                    NowPlayingTrackChanged(nowPlayingTrack);
            }
        }

        private RepeatModes repeatMode = RepeatModes.NONE;
        public RepeatModes RepeatMode
        {
            get => repeatMode;
            set
            {
                repeatMode = value;
                NotifyPropertyChanged(nameof(RepeatMode));
            }
        }

        public PlaybackQueue(List<Track> tracks, bool shuffled = false) =>
            InitializeQueue(tracks[0], tracks, shuffled);

        public PlaybackQueue(Track track, List<Track> tracks, bool shuffled = false) =>
            InitializeQueue(track, tracks, shuffled);

        private void InitializeQueue(Track track, List<Track> tracks, bool shuffled = false)
        {
            Tracks = new ObservableCollection<Track>();
            foreach (var t in tracks) Tracks.Add(t);

            nowPlayingTrackIndex = tracks.IndexOf(track);
            NowPlayingTrack = track;
            ShuffleIsEnabled = shuffled;
            OpenedListHashCode = tracks.GetHashCode();
        }

        public void ToggleShuffle() => ShuffleIsEnabled = !ShuffleIsEnabled;

        public void SwitchRepeatMode()
        {
            switch (RepeatMode)
            {
                case RepeatModes.NONE:
                    RepeatMode = RepeatModes.REPEAT_ALL;
                    break;
                case RepeatModes.REPEAT_ALL:
                    RepeatMode = RepeatModes.REPEAT_ONE;
                    break;
                case RepeatModes.REPEAT_ONE:
                    RepeatMode = RepeatModes.NONE;
                    break;
            }
        }

        public void SkipToTrack(Track track)
        {
            nowPlayingTrackIndex = Tracks.IndexOf(track);
            NowPlayingTrack = track;
        }

        public void SkipToNextTrack(bool forced = false)
        {
            switch (RepeatMode)
            {
                case RepeatModes.NONE:
                    nowPlayingTrackIndex++;
                    break;
                case RepeatModes.REPEAT_ALL:
                    if (nowPlayingTrackIndex == Tracks.Count - 1)
                        nowPlayingTrackIndex = 0;
                    else
                        nowPlayingTrackIndex++;
                    break;
                case RepeatModes.REPEAT_ONE:
                    if (forced)
                    {
                        if (nowPlayingTrackIndex == Tracks.Count - 1)
                            nowPlayingTrackIndex = 0;
                        else
                            nowPlayingTrackIndex++;
                    }
                    break;
            }

            NowPlayingTrack = Tracks[nowPlayingTrackIndex];
        }

        public void SkipToPreviousTrack()
        {
            if (Tracks.Count > 1)
            {
                switch (RepeatMode)
                {
                    case RepeatModes.NONE:
                        if (nowPlayingTrackIndex != 0)
                            nowPlayingTrackIndex--;
                        break;
                    case RepeatModes.REPEAT_ALL:
                        if (nowPlayingTrackIndex == 0)
                            nowPlayingTrackIndex = Tracks.Count - 1;
                        else
                            nowPlayingTrackIndex--;
                        break;
                    case RepeatModes.REPEAT_ONE:
                        break;
                }
            }

            NowPlayingTrack = Tracks[nowPlayingTrackIndex];
        }

        public bool CanSkipToNextTrack()
        {
            if (RepeatMode == RepeatModes.NONE && nowPlayingTrackIndex >= Tracks.Count - 1)
                return false;
            else
                return true;
        }

        public void Shuffle()
        {
            unshuffledQueueTrackIds = new List<int>();
            foreach (Track track in Tracks)
                unshuffledQueueTrackIds.Add(track.Id);

            Tracks.Shuffle();
            Tracks.Remove(NowPlayingTrack);
            Tracks.Insert(0, NowPlayingTrack);
            nowPlayingTrackIndex = 0;
        }

        public void Unshuffle()
        {
            if ((unshuffledQueueTrackIds != null) && (Tracks.Count == unshuffledQueueTrackIds.Count))
            {
                // https://ppolyzos.com/2016/01/29/c-sort-one-collection-based-on-another-one/
                var sorted = unshuffledQueueTrackIds.Join
                (
                    Tracks,
                    key => key,
                    t => t.Id,
                    (key, iitem) => iitem
                )
                .ToList();

                for (int i = 0; i < sorted.Count; i++)
                    Tracks[i] = sorted[i];

                nowPlayingTrackIndex = Tracks.IndexOf(NowPlayingTrack);

                unshuffledQueueTrackIds = null;
            }
        }

        public Track this[int index]
        {
            get => Tracks[index];
        }

        public enum RepeatModes
        {
            NONE,
            REPEAT_ALL,
            REPEAT_ONE
        }
    }
}
