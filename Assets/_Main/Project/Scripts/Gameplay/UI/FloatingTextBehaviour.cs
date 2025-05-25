using Cysharp.Threading.Tasks;
using UnityEngine;

public class FloatingTextBehaviour : MonoBehaviour
{

    [SerializeField] private TextMesh tmp;
    
    private void Awake()
    {
        tmp.text = "";
    }

    public void SetText(string value, Vector3 position)
    {
        tmp.text = value;
        //tmp.material.color = color;
        transform.position = position;
        Destroy().Forget();
    }

    private async UniTask Destroy()
    {
        await UniTask.WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void SetDisable()
    {
        gameObject.SetActive(false);
    }
}