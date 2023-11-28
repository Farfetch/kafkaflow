import React from "react";

export function Resource({ title, description, link, type, date, authors }) {
  return (
    <div class="card-demo padding-top--md">
      <div class="card">
        <div class="card__header">
          <h3>
            {type}: {title}
          </h3>
        </div>
        <div class="card__body">
          <p>
            <small>
              By: 
              {authors.map(({ name, url }, index) => (
                <>
                  <span>{index ? ", " : ""}</span>
                  <a href={url} title={name}>
                    {name}
                  </a>
                </>
              ))}
              <br />
              {date}
            </small>
          </p>
          {description ? <p>{description}</p> : <></>}
          <p></p>
          <a title={title} href={link}>
            {link}
          </a>
        </div>
      </div>
    </div>
  );
}
