using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Game.UI
{
    public class HUDUi : BaseUI
    {
        public TMP_Text score;
        public TMP_Text combo;
        public override void Setup()
        {
        }


        public void UpdateScore(int _score)
        {
            score.text = _score.ToString();
        }

        public void UpdateCombo(int _combo)
        {
            combo.text = _combo.ToString();
        }
        
    }
}
