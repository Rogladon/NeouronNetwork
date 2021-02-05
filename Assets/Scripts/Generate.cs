using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generate : MonoBehaviour
{
	public GameObject prefabBactery;
	public GameObject prefabFood;
	public int timeEpoch;
	public int countFoods;

	public float _timer;
	private int epoch = 0;
	public float speedTime = 1;
	public Text text;

	public List<Vector3> foods;
	public List<BacteryControll> bacteris;

	public InputField inputField;
	private Vector2 g;
	private Vector2 h;
	// Start is called before the first frame update
	void Awake() {
		g = new Vector2(-4, 4);
		h = new Vector2(-7, 7);
		for (int i = 0; i < countFoods; i++) {
			GameObject go = Instantiate(prefabFood);
			go.transform.position = new Vector3(Random.Range(h.x, h.y), Random.Range(g.x, g.y), 0);
			foods.Add(go.transform.position);
		}
		InvokeRepeating("Timer", 0, 1);
	}

	private void FixedUpdate() {
		if(_timer > timeEpoch) {
			EndEpoch();
		}
		text.text = epoch.ToString();
	}

	void EndEpoch() {
		_timer = 0;
		epoch++;
		BacteryControll[] parents = FitnessFunction();
		var bs = Copy(bacteris, parents);
		foreach(var i in bacteris) {
			i.gameObject.SetActive(true);
			i.Burn();
		}
		foreach (var i in bs) {
			//i.nn = new NeuronNetwork(parents[0].nn);
			i.nn.Selection(parents[0].nn, parents[1].nn);
			i.nn.Mutation(Random.Range(i.rateMin, i.rateMax));
		}
		Debug.Log(epoch + ": " + parents[0].Name + " и " + parents[1].Name + " Дали потомство");
	}

	BacteryControll[] FitnessFunction2() {
		var bs = Copy(bacteris);
		BacteryControll[] parent = new BacteryControll[2];
		float dist = 1000;
		foreach(var i in bs) {
			float lDist = Vector3.Distance(i.transform.position, i.pointSelf.position);
			if (dist > lDist) {
				dist = lDist;
				parent[0] = i;
			}
		}
		dist = 1000;
		bs.Remove(parent[0]);

		foreach (var i in bs) {
			float lDist = Vector3.Distance(i.transform.position, i.pointSelf.position);
			if (dist > lDist) {
				dist = lDist;
				parent[1] = i;
			}
			Debug.Log(dist);
		}
		return parent;
	}


	BacteryControll[] FitnessFunction() {
		var bs = Copy(bacteris);
		BacteryControll[] parents = new BacteryControll[2];
		//List<BacteryControll> maxLife = MaxTimeLife(bs);
		//if(maxLife.Count == 1) {
		//	bs.Remove(maxLife[0]);
		//	var maxLife2 = MaxTimeLife(bs);
		//	parents[0] = maxLife[0];
		//	parents[1] = maxLife2[0];
		//	return parents;
		//}
		//if(maxLife.Count == 2) {
		//	parents[0] = maxLife[0];
		//	parents[1] = maxLife[1];
		//	return parents;
		//}
		//if(maxLife.Count > 2) {
		var maxScore = MaxScore(bs);
		if (maxScore.Count == 1) {
			bs.Remove(maxScore[0]);
			var maxScore2 = MaxScore(bs);
			parents[0] = maxScore[0];
			parents[1] = maxScore2[0];
			return parents;
		} else {
			parents[0] = maxScore[Random.Range(0, maxScore.Count)];
			maxScore.Remove(parents[0]);
			parents[1] = maxScore[Random.Range(0, maxScore.Count)];
			return parents;
		}

		//}
		return parents;
	}

	List<BacteryControll> Copy(List<BacteryControll> b, List<BacteryControll> bb) {
		List<BacteryControll> bs = new List<BacteryControll>();
		foreach(var i in b) {
			bool c = true;
			foreach(var j in bb) {
				if(i == j) {
					c = false;
				}
			}
			if(c)
				bs.Add(i);
		}
		return bs;
	}

	List<BacteryControll> Copy(List<BacteryControll> b) {
		return Copy(b, new List<BacteryControll>());
	}

	List<BacteryControll> Copy(List<BacteryControll> b, BacteryControll[] bb) {
		List<BacteryControll> bs = new List<BacteryControll>();
		foreach(var i in bb) {
			bs.Add(i);
		}
		return Copy(b, bs);
	}

	List<BacteryControll> MaxTimeLife(List<BacteryControll> b) {
		int time = -1;
		List<BacteryControll> bs = new List<BacteryControll>();

		foreach(var i in b) {
			if(time < i.globalTimeLife) {
				time = i.globalTimeLife;
				bs.Add(i);
			} else if(time == i.globalTimeLife) {
				bs.Add(i);
			}
		}
		return bs;
	}

	List<BacteryControll> MaxScore(List<BacteryControll> b) {
		int score = -1;
		List<BacteryControll> bs = new List<BacteryControll>();

		foreach (var i in b) {
			if (score < i.score) {
				score = i.score;
				bs.Add(i);
			} else if (score == i.score) {
				bs.Add(i);
			}
		}
		return bs;
	}

	void Timer() {
		_timer++;
	}



	public void RemoveFood(GameObject food) {
		foods.Remove(food.transform.position);
		Destroy(food);
		GameObject go = Instantiate(prefabFood);
		go.transform.position = new Vector3(Random.Range(h.x, h.y), Random.Range(g.x, g.y), 0);
		foods.Add(go.transform.position);

	}

	public void SetTimeSpeed() {
		int b;
		if (int.TryParse(inputField.text, out b))
			Time.timeScale = b;
	}

	//// Update is called once per frame
	//void FixedUpdate()
	//   {
	//	if (_timer >= timeEpoch) {
	//		_timer = 0;
	//		EndEpoch();
	//	}
	//}

	//public void DiedBactary() {
	//	bool a = true;
	//	foreach(var i in bacteris) {
	//		if (i.gameObject.activeSelf) {
	//			a = false;
	//		}
	//	}
	//	if (a) {
	//		EndEpoch();
	//	}
	//}

	//public void SetTimeSpeed() {
	//	int speed;
	//	if(int.TryParse(inputField.text,out speed)) {
	//		speedTime = speed;
	//		foreach (var i in bacteris) {
	//			i.InvokeRepeating("Action", 1, Time.fixedDeltaTime / speedTime);
	//		}
	//	}
	//}

	//public void EndEpoch() {
	//	float maxLife = 0;
	//	int indexLife = -1;
	//	//List<BacteryControll> sucBact = new List<BacteryControll>();
	//	//for (int i = 0; i < bacteris.Count; i++) {
	//	//	if (maxLife < bacteris[i].timeLife && bacteris[i].score != 0) {
	//	//		maxLife = bacteris[i].timeLife;
	//	//		indexLife = i;
	//	//	}
	//	//	if(maxLife == bacteris[i].timeLife) {
	//	//		sucBact.Add(bacteris[i]);
	//	//	}
	//	//	bacteris[i].timeLife = 0;
	//	//}
	//	//if (indexLife == -1) {
	//	//	indexLife = Random.Range(0, bacteris.Count);
	//	//}
	//	if (bacteris.Count > 0) {
	//		int max = 0;
	//		int index = 0;
	//		for (int i = 0; i < bacteris.Count; i++) {
	//			if (max < bacteris[i].score) {
	//				max = bacteris[i].score;
	//				indexLife = i;
	//			}
	//			bacteris[i].score = 0;
	//		}
	//	}
	//	foreach(var i in bacteris) {
	//		i.transform.position = Vector3.zero;
	//		i.died = false;
	//		i.timeLife = 0;
	//		i.gameObject.SetActive(true);
	//	}
	//	//if (max != 0) {
	//	NeuronNetwork nn = bacteris[indexLife].nn;
	//	for (int i = 0; i < bacteris.Count; i++) {
	//		bacteris[i].nn = new NeuronNetwork(nn);
	//		bacteris[i].Evolution();
	//	}
	//	Debug.Log("Поколение: " + epoch + ". Самая удачливая особь: " + bacteris[indexLife].Name + " дала потомство");
	//	//} else {
	//	//	for (int i = 0; i < bacteris.Count; i++) {
	//	//		bacteris[i].Evolution();
	//	//	}
	//	//	Debug.Log("Все неудачники и лошары. Эволюционируем...");
	//	//}
	//	epoch++;
	//	text.text = epoch.ToString();
	//}


}
