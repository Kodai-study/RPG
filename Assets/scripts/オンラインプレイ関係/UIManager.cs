using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    //�e���̕\�����s���e�L�X�g
    public Text bulletText;


    /// <summary>
    /// �������̏e�̒e�����e�L�X�g�ɔ��f
    /// </summary>
    /// <param name="ammoClip"></param>
    /// <param name="ammunition"></param>
    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        //�e�L�X�g�ɑ������̏e�́@�}�K�W�����e��/�����e���@��\��
        bulletText.text = ammoClip + "/" + ammunition;
    }

}