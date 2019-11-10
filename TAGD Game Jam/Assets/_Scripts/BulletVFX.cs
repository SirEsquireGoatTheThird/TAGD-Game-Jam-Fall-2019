using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    private int pattern_used_count = 0;
    public int pattern_used_target;
    Renderer visible;
    Animator animatior_comp;
    Transform transform_comp;
    public GameObject enemy_container;
    RectTransform enemy_transform;
    Vector3[] enemy_corners;
    Vector3 random_pos;
    float enemy_y_length;
    float enemy_x_length;

    void Start()
    {
        visible = GetComponent<Renderer>();
        animatior_comp = GetComponent<Animator>();
        transform_comp = GetComponent<Transform>();
        enemy_transform = enemy_container.GetComponent<RectTransform>();
        enemy_corners = new Vector3[4];
        enemy_transform.GetWorldCorners(enemy_corners);
        enemy_y_length = Mathf.Abs(enemy_corners[1].y - enemy_corners[0].y);
        enemy_x_length = Mathf.Abs(enemy_corners[3].x - enemy_corners[0].x);
        visible.enabled = false;
        GameManager.Instance.UpdateTimeDuration.AddListener(reset_pattern_count);
        GameManager.Instance.PatternUsed.AddListener(show_effect);
    }

    private void Update()
    {
        if((animatior_comp.GetCurrentAnimatorStateInfo(0).IsName("shoot") && animatior_comp.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9) || animatior_comp.GetCurrentAnimatorStateInfo(0).IsName("Nothing"))
        {
            Debug.Log(pattern_used_count);
            visible.enabled = false;
        }
    }

    void reset_pattern_count()
    {
        pattern_used_count = 0;
    }

    void show_effect()
    {
        pattern_used_count += 1;
        if (pattern_used_target == pattern_used_count)
        {
            if(!visible.enabled)
            {
                random_pos = new Vector3(Random.Range(enemy_x_length/5.0f,enemy_x_length/2.0f),Random.Range(enemy_y_length / 2.0f, 4.0f * enemy_y_length / 5.0f),-.01f);
                transform_comp.position = enemy_corners[0] + random_pos;
                visible.enabled = true;
                animatior_comp.Play("shoot", 0, 0);
            }
        }
    }


}
