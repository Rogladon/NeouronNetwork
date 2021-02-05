using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteryControll : MonoBehaviour
{
	public int score;
	public float rateMax;
	public float rateMin;
	public float speed;
	public int globalTimeLife { get; private set; }
	public int timeLife;
	public int timer;
	public string Name;
	public Transform pointSelf;

	private Vector2 pos { get {
			return new Vector2(transform.position.x, transform.position.y);
		}
		set {
			transform.position = value;
		}
		}
	private bool death;
	public NeuronNetwork nn { get; set; }
	public Generate generate;

	private Vector2 startPos;

	private void Start() {
		startPos = pos;
		Burn();
		generate.bacteris.Add(this);
		nn = new NeuronNetwork(1, new int[] { generate.countFoods*2+2,30,20,14, 2 });
	}

	public void Burn() {
		//pos = startPos;
		InvokeRepeating("Timer", 0, 1);
		death = false;
		timer = 0;
		score = 0;
	}

	private void FixedUpdate() {
		if (death) return;
		Vector2 point = FeedForwards();
		MoveDir(point);
		if(timer > timeLife) {
			Death();
		}
	}

	void Death() {
		death = true;
		CancelInvoke();
		gameObject.SetActive(false);
	}

	void Timer() {
		timer += 1;
		globalTimeLife += 1;
	}

	void Move(Vector2 point) {
		point.x = point.x * 8;
		point.y = point.y * 5;
		Vector2 dir = (point - pos).normalized;
		pos += dir * Time.fixedDeltaTime * speed;
	}

	private void MoveDir(Vector3 dir) {
		dir = dir.normalized;
		pos += (Vector2)dir * speed * Time.fixedDeltaTime; 
	}

	private void MoveRoot(Vector3 dir) {
		dir.x += 0.001f;
		if (!float.IsNaN(dir.x) && !float.IsInfinity(dir.x))
			transform.rotation = Quaternion.Euler(0, 0, dir.x * 360);
		transform.position += transform.right * speed * Time.fixedDeltaTime;
	}

	Vector2 output;


	Vector2 FeedForwards() {
		List<Vector3> foods = generate.foods;
		float[] inputs = new float[generate.countFoods*2 + 2];
		int index = 0;
		if (foods.Count == generate.countFoods) {
			for (int i = 0; i < foods.Count; i++) {
				inputs[index] = foods[i].x;
				index++;
				inputs[index] = foods[i].y;
				index++;
			}
			inputs[index] = pos.x;
			inputs[index] = pos.y;
			float[] outputs = nn.FeedForward(inputs);
			output = new Vector2(outputs[0], outputs[1]);
			
		}
		return output;
	}
	Vector2 FeedForwards2() {
		var foods = generate.foods;
		float[] inputs = new float[4];
		inputs[0] = pos.x;
		inputs[1] = pos.y;
		inputs[2] = pointSelf.position.x;
		inputs[3] = pointSelf.position.y;

		float[] outputs = nn.FeedForward(inputs);
		output = new Vector2(outputs[0],outputs[1]);
		return output;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Food")) {
			timer = 0;
			score++;
			generate.RemoveFood(collision.gameObject);
		} else if (collision.CompareTag("PortalX")) {
			pos = new Vector2(-Mathf.Sign(pos.x) * (Mathf.Abs(pos.x) - 1), pos.y);
		} else if (collision.CompareTag("PortalY")) {
			pos = new Vector2(pos.x, -Mathf.Sign(pos.y) * (Mathf.Abs(pos.y) - 1));
		}
	}


	//public bool type = false;
	//public int score;
	//public float speed;
	//public float rate;
	//public string Name;

	//public Generate generate;
	//public NeuronNetwork nn;

	//public Vector3 dir;
	//public bool died = false;
	//   // Start is called before the first frame update
	//   void Start()
	//   {
	//	generate.bacteris.Add(this);
	//	nn = new NeuronNetwork(rate, new int[] { 22,20, 10, 1 });
	//	Respawn();

	//}

	//   // Update is called once per frame
	//   void Update()
	//   {

	//   }
	//public void Respawn() {

	//	List<Vector3> foods = generate.foods;
	//	float[] inputs = new float[22];
	//	int index = 0;
	//	for (int i = 0; i < 8; i++) {
	//		inputs[index] = foods[i].x;
	//		index++;
	//		inputs[index] = foods[i].y;
	//		index++;
	//	}
	//	inputs[index] = transform.position.x;
	//	index++;
	//	inputs[index] = transform.position.y;
	//	index++;
	//	inputs[index] = -7f;
	//	index++;
	//	inputs[index] = 7f;
	//	index++;
	//	inputs[index] = -4f;
	//	index++;
	//	inputs[index] = 4f;

	//	float[] outputs = nn.FeedForward(inputs);
	//	dir = new Vector3(outputs[0],0, 0);
	//	//Debug.Log(Name + " " + dir);
	//	Move(dir);
	//}
	//private void Action() {
	//	if (died) return;
	//	timeLife++;
	//	Respawn();
	//	Move(dir);
	//	generate._timer++;
	//	if(timeLife >= MaxTimeLife && type) {
	//		Died();
	//	}
	//}
	//private void Move(Vector3 dir) {
	//	dir.x += 0.001f;
	//	if(!float.IsNaN(dir.x) && !float.IsInfinity(dir.x))
	//	transform.rotation = Quaternion.Euler(0, 0, dir.x*360);
	//	transform.position += transform.right * speed * Time.fixedDeltaTime;
	//}
	//private void OnTriggerStay2D(Collider2D other) {
	//	if (other.CompareTag("Food")) {
	//		score++;
	//		if (type) {
	//			timeLife = 0;
	//		}
	//		generate.RemoveFood(other.gameObject);
	//	} 

	//}

	//public void Died() {
	//	gameObject.SetActive(false);
	//	died = true;
	//	generate.DiedBactary();
	//}

	//public void Evolution() {
	//	nn.EvolutionAlg();
	//}
	//public float timeLife = 0;
	//public float MaxTimeLife;
	//private void OnCollisionStay2D(Collision2D other) {
	//	if (other.gameObject.CompareTag("Died")) {
	//		Died();
	//		//Debug.Log(Name + " Obstacle");
	//	}
	//}
}

/*
 * несколько особей, на каждую эпоху выделенно определенное количсевто времени,
 * на вход подается все координаты еды, на выход координата к которой нужно двигаться,
 * в конце эпохи выбирается 2 лучшие особи и скрещиваются
 * потомки получают случайные части весов родетелей, после
 * чего потомки мутируют со случайной силой мутации
 * всего в новой популяции 3 потомка и оба родителя, остальные уничтожаются
 * 
 * лучшие особи - особи которые дольше всех прожили
 * есть время жизни без еды, при съедании время продлевается
 * Если время жизни одинаково, лучшие те кто съел больше еды
 * 
 * 
 */