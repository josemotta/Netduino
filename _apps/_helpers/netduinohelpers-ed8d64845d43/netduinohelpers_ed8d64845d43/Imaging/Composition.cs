using System;
using System.Collections;

namespace netduino.helpers.Imaging {
    public class Composition {
        private readonly ArrayList _missiles = new ArrayList();
        private byte[] _frameCache;
        private int _frameCacheX;
        private int _frameCacheY;

        public Composition(Bitmap background) {
            Background = background;
        }

        public Composition(byte[] background, int width, int height)
            : this(new Bitmap(background, width, height)) {}

        public Bitmap Background { get; private set; }

        public PlayerMissile this[string name] {
            get {
                foreach (PlayerMissile missile in _missiles) {
                    if (missile.Name == name) return missile;
                }
                return null;
            }
        }

        public event CoincEventHandler Coinc;

        protected virtual void OnCoinc(CoincEventArgs e) {
            if (Coinc != null) {
                Coinc(this, e);
            }
        }

        public PlayerMissile AddMissile(
            string name,
            int x = 0,
            int y = 0) {
            var missile = new PlayerMissile(
                name: name,
                x: x,
                y: y,
                owner: this
                );
            _missiles.Add(missile);
            ClearCache();
            return missile;
        }

        public PlayerMissile AddMissile(PlayerMissile missile) {
            _missiles.Add(missile);
            missile.Owner = this;
            ClearCache();
            return missile;
        }

        private void ClearCache() {
            _frameCache = null;
        }

        public void RemoveMissile(string name) {
            foreach (PlayerMissile missile in _missiles) {
                if (missile.Name == name) _missiles.Remove(missile);
                missile.Owner = null;
            }
            ClearCache();
        }

        public byte[] GetFrame(int offsetX, int offsetY) {
            if (_frameCache != null && offsetX == _frameCacheX && offsetY == _frameCacheY) {
                return _frameCache;
            }
            _frameCache = Background.GetFrame(offsetX, offsetY);
            _frameCacheX = offsetX;
            _frameCacheY = offsetY;

            foreach (PlayerMissile missile in _missiles) {
                var relX = missile.X - offsetX;
                var relY = missile.Y - offsetY;
                if (relY < 0 || relX < 0 || relX >= Bitmap.FrameSize || relY >= Bitmap.FrameSize) continue;
                _frameCache[relY] |= Bitmap.ShiftMasks[relX];
            }

            CheckForCollisions();
            
            return _frameCache;
        }

        private void CheckForCollisions() {
            for (var i = 0; i < _missiles.Count; i++) {
                var missile1 = (PlayerMissile)_missiles[i];
                for (var j = i + 1; j < _missiles.Count; j++) {
                    var missile2 = (PlayerMissile)_missiles[j];
                    if (missile1.X == missile2.X && missile1.Y == missile2.Y) {
                        OnCoinc(new CoincEventArgs(missile1, missile2));
                    }
                }
            }
        }

        public void NotifyChange() {
            ClearCache();
        }
    }
}