
const couchbase = require('couchbase')
const fs = require('fs')


class MoviesFramework {

  constructor(hostname, username, password, bucket_name) {

    this.cluster = new couchbase.Cluster('couchbase://' + hostname,
      {
        username: username,
        password: password
      })
    this.bucket = this.cluster.bucket(bucket_name)
  }

  async add_rating(rating_id, movie_id, user_id, value) {

    var rating_json = { movie_id: movie_id, user_id: user_id, value: value }

    var answer = await this.bucket.collection('ratings').upsert(rating_id, rating_json).catch((reason) => console.log(reason));
    if (answer) {
      console.log('OK')
    }
  }

  async get_rating(rating_id) {

    var answer = await this.bucket.collection('ratings').get(rating_id).catch((reason) => console.log(reason));
    if (answer) {
      console.log(answer.value)
    }
  }

  async delete_rating(rating_id) {

    var answer = await this.bucket.collection('ratings').remove(rating_id).catch((reason) => console.log(reason));
    if (answer) {
      console.log('OK')
    }
  }

  async get_user_country(user_id) {

    var answer = await this.bucket.collection('users').lookupIn(user_id, [couchbase.LookupInSpec.get('country')]).catch((reason) => console.log(reason));
    if (answer) {
      answer.results.forEach((result) => {
        console.log(result.value)
      })
    }
  }

  async update_user_country(user_id, country) {

    var answer = await this.bucket.collection('users').mutateIn(user_id, [couchbase.MutateInSpec.upsert('country', country)]).catch((reason) => console.log(reason));
    if (answer) {
      console.log('OK')
    }
  }

  async delete_movie(movie_id) {

    var answer_single = await this.cluster.query('DELETE FROM `rate-these-movies` USE KEYS $1', { parameters: [movie_id] }).catch((reason) => console.log(reason));
    var answer_linked = await this.cluster.query('DELETE FROM `rate-these-movies` WHERE id_movie=$1', { parameters: [movie_id] }).catch((reason) => console.log(reason));
    if (answer_single && answer_linked) console.log('OK')
  }

  async list_top_5() {

    var answer = await this.cluster.query('SELECT a.name AS name, AVG(b.`value`) AS avg FROM `rate-these-movies` AS a JOIN `rate-these-movies` AS b ON META(a).id=b.id_movie GROUP BY a.name ORDER BY avg DESC LIMIT 5').catch((reason) => console.log(reason));
    if (answer) {
      answer.rows.forEach((row) => {
        console.log(row.name + ' -> ' + row.avg.toFixed(2))
      })
    }
  }
}


var mf = new MoviesFramework("192.168.1.10", "Administrator", "123456", "rate-these-movies")

// Core operations

//mf.add_rating("rating_98", "movie_0", "user_20", 10)
//mf.get_rating("rating_98")
//mf.delete_rating("rating_98")

// Sub-document operations

//mf.get_user_country("user_0")
//mf.update_user_country("user_0", "Canada")

// N1QL queries

//mf.delete_movie('movie_9')
//mf.list_top_5()
