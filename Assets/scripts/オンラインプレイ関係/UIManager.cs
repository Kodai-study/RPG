using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    //弾数の表示を行うテキスト
    public Text bulletText;


    /// <summary>
    /// 装備中の銃の弾数をテキストに反映
    /// </summary>
    /// <param name="ammoClip"></param>
    /// <param name="ammunition"></param>
    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        //テキストに装備中の銃の　マガジン内弾数/所持弾数　を表示
        bulletText.text = ammoClip + "/" + ammunition;
    }

}