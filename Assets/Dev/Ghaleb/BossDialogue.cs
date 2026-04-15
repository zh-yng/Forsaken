using UnityEngine;
using TMPro;
using System.Collections;

public class BossDialogue : MonoBehaviour
{

		public BossStateMachine stateMachine;
		public GameObject text;
		public TextMeshProUGUI textField;

		int atkPrev = 0;
		int stunCount = 0; 

    // Update is called once per frame
    void Update() {
				if (stateMachine.IsStunned){
						stunCount++;
						PrintText(true);
				}
				if (stateMachine.AttackFinished == 0 && atkPrev == 1) {
					atkPrev = 0;
				}
				if (stateMachine.AttackFinished == 1 && atkPrev == 0) {
					PrintText(false);
				}

    }

		void CloseText() {
			text.SetActive(false);
		}

		void PrintText(bool stunned) {
			string a = "p";
			if (stunned) {
				if (stunCount == 1) {
					a = "stunOne test";
				} else if (stunCount == 2) {
					a = "stunTwo test";
				} else if (stunCount == 3) {
					a = "stunThree test";
				}
				textField.text = a;
				text.SetActive(true);
				Invoke("CloseText", 2f);
			} else {
					int dialougeChoice = Random.Range(0,5);
					if (dialougeChoice == 0) {
						a = "\"This must be done...\"";
					} else if (dialougeChoice == 1) {
						a = "placeholder";
					} else if (dialougeChoice == 2) {
						a = "placeholder";
					} else if (dialougeChoice == 3) {
						a = "placeholder";
					} else if (dialougeChoice == 4) {
						a = "placeholder";
					}
					atkPrev = 1;
					textField.text = a;
					text.SetActive(true);
					CancelInvoke("CloseText");
					Invoke("CloseText", 2f);
			}
		}
}
