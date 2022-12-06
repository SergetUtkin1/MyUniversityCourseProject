using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Figure
{
    [SerializeField] private GameObject _gameObject;
    public Sprite Face { get; set; }
    public Transform Transform { get; set; }
}
