using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeController : MonoBehaviour
{
    IBehavior behaviorTreeRoot;

    // Start is called before the first frame update
    void Start()
    {
        //  initialize root object
        SelectorNode selectorRoot = new SelectorNode();

        //  assign the behaviorTreeRoot to our initialized object
        behaviorTreeRoot = selectorRoot;

        //  initialize children of the root
        //  left branch
        SelectorNode selector = new SelectorNode();
        //  initialize the children of this node:
        demoQuestion question = new demoQuestion();
        demoAction action = new demoAction();

        //  add the children of this node to the parent
        selector.childBehaviors.Add(question);
        selector.childBehaviors.Add(action);

        //  right branch
        SequenceNode sequence = new SequenceNode();
        //  initialize the children of this node
        demoQuestion sequenceQuestion = new demoQuestion();
        demoAction sequenceAction = new demoAction();

        //  add the children of this node to the parent
        sequence.childBehaviors.Add(sequenceQuestion);
        sequence.childBehaviors.Add(sequenceAction);

        //  add the selector and sequence behaviors to the selector root obj
        selectorRoot.childBehaviors.Add(selector);
        selectorRoot.childBehaviors.Add(sequence);


    }

    // Update is called once per frame
    void Update()
    {
        //  no need for a while loop (the call stack will control the state)
        behaviorTreeRoot.DoBehavior(this.GetComponent<Unit>());
    }
}
