

using TMPro;

namespace Game.UI
{
    public class LevelCompleteUi : BaseUI
    {
        public TMP_Text score;
        public override void Setup()
        {
        }


        public void UpdateScore(int _score)
        {
            score.text = _score.ToString();
        }
    }
}
