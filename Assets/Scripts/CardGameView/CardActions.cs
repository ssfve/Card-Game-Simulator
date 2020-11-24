/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using CardGameView.Multiplayer;
using Cgs;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGameView
{
    public delegate void CardAction(CardModel cardModel);

    [PublicAPI]
    public static class CardActions
    {
        public static IReadOnlyDictionary<CardGameDef.CardAction, CardAction> ActionsDictionary =>
            _actionsDictionary ?? (_actionsDictionary = new Dictionary<CardGameDef.CardAction, CardAction>
            {
                [CardGameDef.CardAction.Flip] = Flip,
                [CardGameDef.CardAction.Rotate] = Rotate,
                [CardGameDef.CardAction.Tap] = Tap
            });

        private static Dictionary<CardGameDef.CardAction, CardAction> _actionsDictionary;

        public static void Flip(CardModel cardModel)
        {
            cardModel.IsFacedown = !cardModel.isFacedown;
            EventSystem.current.SetSelectedGameObject(null, cardModel.CurrentPointerEventData);
        }

        public static void Rotate(CardModel cardModel)
        {
            if (cardModel.IsOnline && !cardModel.hasAuthority)
            {
                Debug.LogWarning("Attempted to rotate card without authority!");
                return;
            }

            cardModel.transform.rotation *= Quaternion.Euler(0, 0, -CardGameManager.Current.GameCardRotationDegrees);
            if (cardModel.IsOnline)
                cardModel.CmdUpdateRotation(cardModel.transform.rotation);
        }

        public static void Tap(CardModel cardModel)
        {
            if (cardModel.IsOnline && !cardModel.hasAuthority)
            {
                Debug.LogWarning("Attempted to rotate card without authority!");
                return;
            }

            bool isVertical = cardModel.transform.rotation.Equals(Quaternion.identity);
            cardModel.transform.rotation = isVertical
                ? Quaternion.AngleAxis(CardGameManager.Current.GameCardRotationDegrees, Vector3.back)
                : Quaternion.identity;
            if (cardModel.IsOnline)
                cardModel.CmdUpdateRotation(cardModel.transform.rotation);
        }
    }
}
