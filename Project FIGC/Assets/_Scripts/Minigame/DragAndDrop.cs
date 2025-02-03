using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 _dragOffset; //Distancia mouse - pivote
    private Camera _cam; //Busca el objeto de la camara
    [SerializeField] private float _speed = 10; //Velocidad a la que el objeto sigue al mouse una vez seleccionado+

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite wateringSprite;
    [SerializeField] Sprite idleSprite;

    [SerializeField] private Transform resetPosition;


    public bool isActive = false;

    private void Awake() // Funcion que se llama al momento de cargar la intancia del script
    {
        _cam = Camera.main; //Objeto camara
    }

    private void Start()
    {
        _spriteRenderer.sprite = idleSprite;
    }
    void OnMouseDown() // Funcion que cuando el objeto es seleccionado calcula la distancia entre el punto pivote y el mouse
    {
        _dragOffset = transform.position - GetMousePos();
    }


    private void OnMouseDrag()
    {
        //transform.position = GetMousePos() + _dragOffset;
        transform.position = Vector3.MoveTowards(transform.position, GetMousePos() + _dragOffset, _speed * Time.deltaTime);  //La posicion del objeto es igual a Vector3.Movetowards

    }

    Vector3 GetMousePos() //Funcion que regresa un Vector3 de la posicion del mouse
    {
        //var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition); //La posicion del mouse es igual a la posicion relativa(x, y) en la pantalla desplegada y la convierta en cordenadas de posición
        mousePos.z = 0; //No tiene eje z 
        return mousePos; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other != null)
        {
          
            _spriteRenderer.sprite = wateringSprite;
            isActive = true;
                 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            _spriteRenderer.sprite = idleSprite;
            isActive = false;
        }
    }
}
