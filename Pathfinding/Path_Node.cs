﻿using UnityEngine;
using System.Collections;

public class Path_Node<T> {

	public T data;

	public Path_Edge<T>[] edges; // nodes which lead OUT of this node


}
