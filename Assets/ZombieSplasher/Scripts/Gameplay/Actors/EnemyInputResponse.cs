﻿using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class EnemyInputResponse : MonoBehaviour, IPointerClickHandler, IActorInputResponse
{
    public event EventHandler<ActorPropertiesEventArgs> ActorClicked;

    private ActorProperties _properties;

    public void Initialize(ActorProperties actorProperties)
    {
        _properties = actorProperties;
    }

    public void OnActorClicked(ActorPropertiesEventArgs e)
    {
        if (ActorClicked != null)
        {
            ActorClicked(this, e);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //FCB.EventSystem.EventManager.Instance.Raise(new ActorClickedEvent(gameObject, _properties));
        OnActorClicked(new ActorPropertiesEventArgs(gameObject, _properties));
    }
}
