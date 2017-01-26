using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SylphyHorn.Services
{
    public class ShortcutKeyAccumulator
    {
        private readonly HashSet<Keys> _pressedModifiers = new HashSet<Keys>();
        private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();

        public IReadOnlyCollection<Keys> Modifiers => this._pressedModifiers;

        public IReadOnlyCollection<Keys> Keys => this._pressedKeys;


        public void Add(Keys keyCode)
        {
            if (keyCode.IsModifyKey())
            {
                this._pressedModifiers.Add(keyCode);
            }
            else
            {
                this._pressedKeys.Add(keyCode);
            }
        }

        public void Remove(Keys keyCode)
        {
            if (this._pressedModifiers.Contains(keyCode) && keyCode.IsModifyKey())
            {
                this._pressedModifiers.Remove(keyCode);
            }

            if (this._pressedKeys.Contains(keyCode) && !keyCode.IsModifyKey())
            {
                this._pressedKeys.Remove(keyCode);
            }
        }

        public IEnumerable<ShortcutKey> GetShortcutKeys()
        {
            foreach (var key in this._pressedKeys)
            {
                yield return new ShortcutKey(key, this._pressedModifiers);
            }
        }

        public void Clear()
        {
            this._pressedModifiers.Clear();
            this._pressedModifiers.Clear();
        }
    }
}
