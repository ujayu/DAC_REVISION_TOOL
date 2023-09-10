import React from 'react';
import { Link } from 'react-router-dom';

export default function Module({ module }) {
  return (
    <div className="card m-3 border-secondary bg-danger  ">
      <div className="card-body">
        <div className="float-start">

        <h5 className="card-title text-secondary">
          <Link to={`/module/${module.moduleId}`}>{module.moduleName}</Link>
        </h5>
        <p className="card-text">
        Revision remaining:: {module.pointsWithNextTimeLessThanCurrent}
        </p>
        <p className="card-text">
        Revision completed: {module.pointsWithAskTimeGreaterThanTokenAssignTime}
        </p>
        <p className="card-text">
        For tomorrow: {module.pointsWithNextTimeForTomorrow}
        </p>
        </div>
      </div>
    </div>
  );
}
