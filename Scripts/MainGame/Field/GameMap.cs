using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TimeTraveler.MainGame{
	public class GameMap {
		List<int> prohibitedCellNumbers = new List<int>(){};
		const int CELL_DISTANCE = 100;


		public void Initialize() {
		}

		public int CellNumber(int x, int y) {
			return (x % 10) + (y * 10);
		}

		public void SetCurrentProhibitedArea(List<int> prohibitedCellNums) {
			this.prohibitedCellNumbers = prohibitedCellNums;
		}

		public List<int> NextToHeroRoute(int heroCurrentCellNum) {
			int cellNum;
			if (!prohibitedCellNumbers.Exists (x => x == heroCurrentCellNum + 1)) { // right
				cellNum = heroCurrentCellNum + 1;
			} else if(!prohibitedCellNumbers.Exists (x => x == heroCurrentCellNum - 1)) { // left
				cellNum = heroCurrentCellNum - 1;
			} else if(!prohibitedCellNumbers.Exists (x => x == heroCurrentCellNum - 10)) { // up
				cellNum = heroCurrentCellNum - 10;
			} else if (!prohibitedCellNumbers.Exists (x => x == heroCurrentCellNum + 10)) { // down
				cellNum = heroCurrentCellNum + 10;
			} else {
				throw new Exception("cant move next to hero");
			}
			return this.Route (heroCurrentCellNum, cellNum, false);
		}

		public List<int> Route(int startCellNumber, int endCellNumber, bool isProhibited) {
			if (startCellNumber == endCellNumber) return null;
			List<Node> openNodeList = new List<Node>();
			List<Node> closeNodeList = new List<Node>();
			Vector2 startNodeXY = this.XYpoint(startCellNumber);
			Vector2 endNodeXY = this.XYpoint(endCellNumber);
			Node startNode = new Node(startCellNumber, this.DistancePow(startNodeXY, endNodeXY), null);
			Node endNode = null;
			openNodeList.Add(startNode);
			int cnt = 0;
			while (true){
				cnt++;
				if (cnt > 1000) {
					break;
				}

				Node closestToStartNode = null;
				foreach(Node node in openNodeList){
					if (closestToStartNode == null){
						closestToStartNode = node;
						continue;
					}

					if (node.estimateDistance < closestToStartNode.estimateDistance) {
						closestToStartNode = node;
					}
				}

				if (closestToStartNode.cellNumber == endCellNumber) {
					endNode = closestToStartNode;
					break;
				} 
				else {
					openNodeList.Remove(closestToStartNode);
					closeNodeList.Add (closestToStartNode);
				}

				List<Node> closestNodes = getClosestNodes(closestToStartNode, endNodeXY, endCellNumber);

				foreach(Node closestNode in closestNodes) {
					Node openNode = openNodeList.Find (x => x.cellNumber == closestNode.cellNumber);
					Node closeNode = closeNodeList.Find (x => x.cellNumber == closestNode.cellNumber);
					if (openNode != null) {
						if (openNode.estimateDistance > closestNode.estimateDistance) {
							openNode.estimateDistance = closestNode.estimateDistance;
							openNode.parentNode = closestNode.parentNode;
						}
					}
					else if(closeNode != null) {
						if (closeNode.estimateDistance > closestNode.estimateDistance) {
							closeNode.estimateDistance = closestNode.estimateDistance;
							closeNode.parentNode = closestNode.parentNode;
							closeNodeList.Remove(closeNode);
							openNodeList.Add (closeNode);
						}
					}
					else {
						openNodeList.Add (closestNode);
					}
				}

				if (openNodeList.Count == 0) {
					return null;
				}
			}

			return this.RouteCellNumbers(endNode, isProhibited);
		}

		private class Node {
			public int cellNumber;
			public float distanceToEnd;
			public Node parentNode;
			public float estimateDistance;

			public Node(int cellNumber, float distanceToEnd, Node parentNode) {
				this.cellNumber = cellNumber;
				this.distanceToEnd = distanceToEnd;
				this.parentNode = parentNode;
				if (parentNode != null) {
					this.estimateDistance = parentNode.estimateDistance - parentNode.distanceToEnd + this.distanceToEnd + CELL_DISTANCE;
				}
				else {
					this.estimateDistance = distanceToEnd;
				}

			}
		}

		List<int> RouteCellNumbers (Node goalNode, bool isProhibited) {
			if (goalNode == null) {
				return null;
			}
			List<int> routeCellNumbers = new List<int>();
			routeCellNumbers.Insert(0, goalNode.cellNumber);
			Node prevNode = goalNode.parentNode;
			while (true) {
				routeCellNumbers.Insert(0, prevNode.cellNumber);
				if (prevNode.parentNode == null) {
					break;
				}
				prevNode = prevNode.parentNode;
			}
			return routeCellNumbers;
		}

		public Vector2 XYpoint (int cellNumber) {
			return new Vector2(cellNumber % 10, cellNumber / 10);
		}

		float DistancePow (Vector2 vec1, Vector2 vec2) {
			return Mathf.Pow(vec2.x - vec1.x, 2)+Mathf.Pow(vec2.y - vec1.y, 2) / 10;
		}

		List<Node> getClosestNodes(Node parentNode, Vector2 endNodeXY, int endCellNumber) {
			List<Node> nodes = new List<Node>();
			Vector2 point = this.XYpoint(parentNode.cellNumber);

			if (point.x - 1 >= 0) {
				int cellNumber = parentNode.cellNumber - 1;
				if (!prohibitedCellNumbers.Exists (x => x == cellNumber) || cellNumber == endCellNumber){
					Vector2 xyPoint = this.XYpoint(cellNumber);
					nodes.Add(new Node(cellNumber, this.DistancePow(xyPoint, endNodeXY), parentNode));
				}
			}
			
			if (point.x + 1 < 10) {
				int cellNumber = parentNode.cellNumber + 1;
				if (!prohibitedCellNumbers.Exists (x => x == cellNumber) || cellNumber == endCellNumber){
					Vector2 xyPoint = this.XYpoint(cellNumber);
					nodes.Add(new Node(cellNumber, this.DistancePow(xyPoint, endNodeXY), parentNode));
				}
			}
			
			if (point.y - 1 >= 0) {
				int cellNumber = parentNode.cellNumber - 10;
				if (!prohibitedCellNumbers.Exists (x => x == cellNumber) || cellNumber == endCellNumber){
					Vector2 xyPoint = this.XYpoint(cellNumber);
					nodes.Add(new Node(cellNumber, this.DistancePow(xyPoint, endNodeXY), parentNode));
				}
			}
			
			if (point.y + 1 < 10) {
				int cellNumber = parentNode.cellNumber + 10;
				if (!prohibitedCellNumbers.Exists (x => x == cellNumber) || cellNumber == endCellNumber){
					Vector2 xyPoint = this.XYpoint(cellNumber);
					nodes.Add(new Node(cellNumber, this.DistancePow(xyPoint, endNodeXY), parentNode));
				}
			}
			return nodes;
		}

	}
}
