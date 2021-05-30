using Player.Core.Utils.Extensions;
using Player.Core.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player.Core.Entities
{
    public class PlaybackQueue : Notifier
    {
        private Track nowPlayingTrack;
        private List<int> unshuffledQueueTrackIds;

        public ObservableCollection<Track> Queue { get; private set; }
        public int NowPlayingTrackIndex { get; private set; }

        private bool isShuffleEnabled = false;
        public bool IsShuffleEnabled
        {
            get => isShuffleEnabled;
            set
            {
                isShuffleEnabled = value;
                NotifyPropertyChanged(nameof(IsShuffleEnabled));

                if (isShuffleEnabled)
                    Shuffle();
                else
                    Unshuffle();
            }
        }

        private RepeatMode mode = RepeatMode.NONE;
        public RepeatMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                NotifyPropertyChanged(nameof(Mode));
            }
        }

        public PlaybackQueue(Track track)
        {
            Queue = new ObservableCollection<Track>();
            NowPlayingTrackIndex = 0;
            Queue.Add(track);
            IsShuffleEnabled = false;
        }

        public PlaybackQueue(ObservableCollection<Track> tracks, bool shuffled = false)
        {
            Queue = tracks;
            NowPlayingTrackIndex = 0;
            IsShuffleEnabled = shuffled;
        }

        public PlaybackQueue(Track track, ObservableCollection<Track> tracks, bool shuffled = false)
        {
            Queue = tracks;
            NowPlayingTrackIndex = Queue.IndexOf(track);
            IsShuffleEnabled = shuffled;
        }

        public Track GetNextTrack()
        {
            switch (Mode)
            {
                case RepeatMode.NONE:
                    NowPlayingTrackIndex++;
                    break;
                case RepeatMode.REPEAT_ALL:
                    if (NowPlayingTrackIndex == Queue.Count - 1)
                        NowPlayingTrackIndex = 0;
                    else
                        NowPlayingTrackIndex++;
                    break;
                case RepeatMode.REPEAT_ONE:
                    break;
            }

            nowPlayingTrack = Queue[NowPlayingTrackIndex];
            return nowPlayingTrack;
        }

        public Track GetPreviousTrack()
        {
            if (Queue.Count > 1)
            {
                switch (Mode)
                {
                    case RepeatMode.NONE:
                        if (NowPlayingTrackIndex != 0)
                            NowPlayingTrackIndex--;
                        break;
                    case RepeatMode.REPEAT_ALL:
                        if (NowPlayingTrackIndex == 0)
                            NowPlayingTrackIndex = Queue.Count - 1;
                        else
                            NowPlayingTrackIndex--;
                        break;
                    case RepeatMode.REPEAT_ONE:
                        break;
                }
            }

            nowPlayingTrack = Queue[NowPlayingTrackIndex];
            return nowPlayingTrack;
        }

        public bool CanSkipToNextTrack()
        {
            if (Mode == RepeatMode.NONE && NowPlayingTrackIndex >= Queue.Count - 1)
                return false;
            else
                return true;
        }

        public void Shuffle()
        {
            unshuffledQueueTrackIds = new List<int>();
            foreach (Track track in Queue)
                unshuffledQueueTrackIds.Add(track.Id);

            Queue.Shuffle();
            Queue.Remove(nowPlayingTrack);
            Queue.Insert(0, nowPlayingTrack);
            NowPlayingTrackIndex = 0;
        }

        public void Unshuffle()
        {
            if ((unshuffledQueueTrackIds != null) && (Queue.Count == unshuffledQueueTrackIds.Count))
            {
                // https://ppolyzos.com/2016/01/29/c-sort-one-collection-based-on-another-one/
                var sorted = unshuffledQueueTrackIds.Join
                (
                    Queue,
                    key => key,
                    t => t.Id,
                    (key, iitem) => iitem
                )
                .ToList();

                for (int i = 0; i < sorted.Count; i++)
                    Queue[i] = sorted[i];

                NowPlayingTrackIndex = Queue.IndexOf(nowPlayingTrack);

                unshuffledQueueTrackIds = null;
            }
        }

        public Track this[int index]
        {
            get => Queue[index];
        }

        public enum RepeatMode
        {
            NONE,
            REPEAT_ALL,
            REPEAT_ONE
        }
    }
}
