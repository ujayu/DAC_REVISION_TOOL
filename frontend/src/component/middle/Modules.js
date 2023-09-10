import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from '../../axios';

import Module from './module/Module';
import { useSelector } from 'react-redux';

export default function Modules() {
  const [moduleData, setModuleData] = useState(null);
  const tokens = useSelector((state) => state.token);

  useEffect(() => {
    // Fetch data from the API
    axios.get('/Main/GetData', {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`
        }
      })
      .then(response => {
        setModuleData(response.data);
      })
      .catch(error => {
        console.error('Error fetching module data:', error);
      });
  }, []);

  if (!moduleData) {
    return <p>Loading...</p>;
  }

  return (
    <>
      <div className="card m-3 border-secondary bg-primary">
      <div className="card-body">
          <div className="float-start">
            <h4 className="card-title"><span className="badge rounded-pill bg-secondary">Modules List</span></h4>
            <div className="card-body">
              <div className="card-text d-inline m-0">
                <p className="text-danger me-3 d-inline">Revision remaining: {moduleData.allModulesData.pointsWithNextTimeLessThanCurrent_AllModules}</p>
                <p className="text-success me-3 d-inline">Revision completed: {moduleData.allModulesData.pointsWithAskTimeGreaterThanTokenAssignTime_AllModules}</p>
                <p className="text-info me-3 d-inline">For tomorrow: {moduleData.allModulesData.pointsWithNextTimeForTomorrow_AllModules}</p>
              </div>
            </div>
          </div>
          { moduleData.allModulesData.pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision !==0 &&
          <Link className="btn btn-success float-end" to={`/point/${moduleData.allModulesData.pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision}`}>Start</Link>
          }
          { moduleData.allModulesData.pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision ===0 &&
          <div className="btn btn-success float-end disabled">Revision Completed</div>
          }
        </div>
      </div>
      {moduleData.moduleDataList.map((module, index) => (
        <Module
          key={index}
          module={module}
        />
      ))}
    </>
  );
}
