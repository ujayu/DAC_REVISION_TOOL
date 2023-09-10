import React, { useState } from 'react';
import axios from './../../../axios';
import { useSelector } from 'react-redux';

export default function Point({ point }) {
  const tokens = useSelector((state) => state.token);
  const [isActive, setIsActive] = useState(point.isActive);

  async function handleCheckboxChange() {
    try {
      const response = await axios.put(`/Main/pointInRevision/${point.pointId}`, null, {
        headers: {
          Authorization: `Bearer ${tokens.accessToken}`,
        },
      });
      setIsActive(response.data.isActive); // Update the local state based on the server response
    } catch (error) {
      console.error('Error updating point status:', error);
    }
  }

  return (
    <div className="container text-center">
      <div className="row">
        <div className="col">
          {point.pointName}
        </div>
        <div className="col">
          <fieldset className="form-group">
            <div className="form-check form-switch">
              <input
                type="checkbox"
                className="btn-check"
                id={point.pointId}
                autoComplete="off"
                checked={isActive}
                onChange={handleCheckboxChange}
              />
              <label className={`btn btn-outline-secondary ${isActive ? 'active' : ''}`} htmlFor={point.pointId}>
                {isActive ? 'Remove' : 'Add'}
              </label>
            </div>
          </fieldset>
        </div>
        <div className="col">
          <button type="button" className="btn btn-danger">Clear</button>
        </div>
      </div>
    </div>
  );
}
