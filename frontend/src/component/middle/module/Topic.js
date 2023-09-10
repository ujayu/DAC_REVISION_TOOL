import React, { useState, useEffect } from 'react';
import axios from './../../../axios';
import Point from './Point'; // Import your Point component
import { useSelector } from 'react-redux';

export default function Topic({ topic }) {
  const tokens = useSelector((state) => state.token);
  const [points, setPoints] = useState([]);

  useEffect(() => {
    async function fetchPointsForTopic() {
      try {
        const response = await axios.get(`/Main/topic/${topic.topicId}`, {
            headers: {
              Authorization: `Bearer ${tokens.accessToken}`,
            },
          });
        setPoints(response.data);
      } catch (error) {
        console.error('Error fetching points:', error);
      }
    }

    fetchPointsForTopic();
  }, [topic.topicId]);

  return (
    <div className="card m-3 border-secondary bg-primary">
      <div className="card-body">
        <h5 className="card-title text-secondary">
          <div className="text-info">{topic.topicName}</div>
        </h5>
        {points.map(point => (
          <Point key={point.pointId} point={point} />
        ))}
      </div>
    </div>
  );
}
