import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import axios from './../../axios'; // Import the Axios configuration
import { useSelector } from 'react-redux';
import Topic from './module/Topic';

export default function Module(props) {
  const [moduleData, setModuleData] = useState(null);
  const { moduleId } = useParams();
  const tokens = useSelector((state) => state.token);

  useEffect(() => {
    // Fetch module data from your API
    async function fetchModuleData() {
      try {
        const response = await axios.get(`/Main/module/${moduleId}`, {
          headers: {
            Authorization: `Bearer ${tokens.accessToken}`,
          },
        });
        const data = response.data;
        setModuleData(data);
      } catch (error) {
        console.error('Error fetching module data:', error);
      }
    }

    fetchModuleData();
  }, [props.moduleId]);

  if (!moduleData) {
    return <div>Loading...</div>;
  }

  return (
    <div className="card m-3 border-secondary bg-danger">
      <div className="card-body">
        <div className="float-start">

        <h5 className="card-title text-primary">
          {moduleData.moduleName}
        </h5>
        <p className="card-text">
          Revision remaining: {moduleData.totalPointsInRevision}
        </p>
        <p className="card-text">
          Revision completed: {moduleData.completedRevisions}
        </p>
        <p className="card-text">
          For tomorrow: {moduleData.revisionsToDoTomorrow}
        </p>
        {/* Display topics */}
        <ul className="list-unstyled">
          {moduleData.topics.map(topic => (
            <li key={topic.topicId}>
              <Topic topic={topic} />
            </li>
          ))}
        </ul>
        </div>
        <Link className="btn btn-success float-end" to="/point/questionId">Start</Link>
      </div>
    </div>
  );
}
